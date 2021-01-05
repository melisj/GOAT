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
        public GameObject tubeEndPrefab;
        public GameObject resourcePackPrefab;
        public GridObjectsList objectList;
        public Grid.Grid grid;

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

        public TubeEnd GetGridEndTube(Vector3 position, int rotation)
        {
            GameObject[] walls = grid.ReturnTile(grid.CalculateTilePositionInArray(position)).WallObjs;
            if(walls[rotation] != null)
                return walls[rotation].GetComponent<TubeEnd>();
            return null;
        }
    }

    public class ResourcepackSaveData : DataContainer, ISaveable
    {
        public List<TubeEndInfo> packInfo = new List<TubeEndInfo>();

        public override void Load(SaveHandler handler)
        {
            ResourcePackSaveHandler resourcePackHandler = (ResourcePackSaveHandler)handler;

            // Disable all current resource packs
            resourcePackHandler.RemoveResourcePacks();
            
            // Enable new resource packs
            foreach (TubeEndInfo pack in packInfo)
            {
                List<Resource> resources = new List<Resource>();
                foreach(int indices in pack.resource)
                {
                    resources.Add(resourcePackHandler.GetResource(indices));
                }

                resourcePackHandler.GetGridEndTube(pack.position, pack.rotation)?.CreateResPacks(resources, pack.amount);
            }
        }

        public override void Save(SaveHandler handler)
        {
            ResourcePackSaveHandler resourcePackHandler = (ResourcePackSaveHandler)handler;

            packInfo.Clear();

            GameObject poolParent = PoolManager.Instance.ReturnPoolParent(resourcePackHandler.tubeEndPrefab.GetInstanceID());
            if (poolParent)
            {
                for (int i = 0; i < poolParent.transform.childCount; i++)
                {
                    packInfo.Add(poolParent.transform.GetChild(i).GetComponent<TubeEnd>().Info);
                }
            }
        }
    }
}

