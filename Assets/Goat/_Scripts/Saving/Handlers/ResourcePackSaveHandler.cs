using Goat.Farming;
using Goat.Pooling;
using Goat.Storage;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Goat.Saving
{
    public class ResourcePackSaveHandler : SaveHandler
    {
        public GameObject resourcePackPrefab;
        public GridObjectsList objectList;

        private void Awake()
        {
            data = new ResourcepackSaveData();
        }

        public void RemoveResourcePacks()
        {
            GameObject poolParent = PoolManager.Instance.ReturnPoolParent(resourcePackPrefab.GetInstanceID());
            if (poolParent != null)
            {
                for (int i = 0; i < poolParent.transform.childCount; i++)
                {
                    PoolManager.Instance.ReturnToPool(poolParent.transform.GetChild(i).gameObject);
                }
            }
        }

        public Resource GetResource(int index)
        {
            return (Resource)objectList.GetObject(index);
        }

        public void SpawnResourcePack(ResourcePackInfo packInfo)
        {
            GameObject packObj = PoolManager.Instance.GetFromPool(resourcePackPrefab, packInfo.position, Quaternion.Euler(packInfo.rotation));
            packObj.GetComponent<ResourcePack>().SetupResPack(GetResource(packInfo.resource));
        }
    }

    public class ResourcepackSaveData : DataContainer, ISaveable
    {
        public List<ResourcePackInfo> packInfo = new List<ResourcePackInfo>();

        public override void Load(SaveHandler handler)
        {
            ResourcePackSaveHandler resourcePackHandler = (ResourcePackSaveHandler)handler;

            // Disable all current resource packs
            resourcePackHandler.RemoveResourcePacks();
            
            // Enable new resource packs
            foreach (ResourcePackInfo pack in packInfo)
            {
                resourcePackHandler.SpawnResourcePack(pack);
            }
        }

        public override void Save(SaveHandler handler)
        {
            ResourcePackSaveHandler resourcePackHandler = (ResourcePackSaveHandler)handler;

            packInfo.Clear();

            GameObject poolParent = PoolManager.Instance.ReturnPoolParent(resourcePackHandler.resourcePackPrefab.GetInstanceID());
            if (poolParent != null)
            {
                for (int i = 0; i < poolParent.transform.childCount; i++)
                {
                    packInfo.Add(poolParent.transform.GetChild(i).GetComponent<ResourcePack>().ResourcePackInfo);
                }
            }
        }
    }
}

