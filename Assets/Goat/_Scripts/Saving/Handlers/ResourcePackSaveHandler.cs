using Goat.Farming;
using Goat.Pooling;
using Goat.Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Saving
{
    public class ResourcePackSaveHandler : SaveHandler
    {
        public GameObject endTubePrefab;
        public GridObjectsList objectList;

        private void Awake()
        {
            data = new ResourcepackSaveData();
        }
    }

    public class ResourcepackSaveData : DataContainer, ISaveable
    {
        public List<ResourcePackInfo> packInfo = new List<ResourcePackInfo>();

        public override void Load(SaveHandler handler)
        {
            ResourcePackSaveHandler resourcePackHandler = (ResourcePackSaveHandler)handler;

            foreach (ResourcePackInfo pack in packInfo)
            {
                resourcePackHandler.SetResourcePack(pack);
            }
        }

        public override void Save(SaveHandler handler)
        {
            ResourcePackSaveHandler resourcePackHandler = (ResourcePackSaveHandler)handler;

            packInfo.Clear();

            GameObject poolParent = PoolManager.Instance.ReturnPoolParent(resourcePackHandler.endTubePrefab.GetInstanceID());
            for(int i = 0; i < poolParent.transform.childCount; i++)
            {
                packInfo.Add(poolParent.transform.GetChild(i).GetComponent<ResourcePack>().PackInfo);
            }
        }
    }
}

