using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI;
using Goat.Storage;
using Goat.Grid.Interactions;
using System.Linq;
using System;

namespace Goat.AI.States
{
    public class SearchForStorageInWarehouse : IState
    {
        Worker worker;
        public bool nothingFound;
        public SearchForStorageInWarehouse(Worker worker)
        {
            this.worker = worker;
        }

        public void Tick()
        {

        }

        private StorageInteractable SetStorageTarget(Dictionary<Resource, int> items, StorageInteractable[] storages)
        {
            if (storages.Length == 0 || storages == null || items == null || items.Count == 0)
            {
                
                //Debug.LogError("No item or storages were found when calling Method: SetStorageTarget !");
                if(worker is StockClerk)
                    worker.ItemsToGet.Clear();

                nothingFound = true;
                return null;
            }

            bool storageFound = false;

            int index = 0;
            for (int j = 0; j < items.Count; j++, index++)
            {
                for (int i = 0; i < storages.Length; i++)
                {
                    if (storages[i].Inventory.Contains(items.ElementAt(j).Key))
                    {
                        Debug.LogFormat("Found container in warehouse: {0}", storages[i].transform.position);
                        storageFound = true;
                        return storages[i];
                    }
                }
            }


            if (!storageFound && worker is WarehouseWorker)
                return storages.First();
            else if (!storageFound && worker is StockClerk)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    Debug.LogWarningFormat("Item: {0} no longer in stock!", items.ElementAt(i).Key.name);
                }
                worker.ItemsToGet.Clear();
            }

            return null;
        }

        public void OnEnter()
        {
            nothingFound = false;
            worker.targetStorage = null;
            Debug.Log("Started searching for containers in warehouse");
            if(worker is StockClerk)
            {
                StorageInteractable[] possibleStorages = worker.storageLocations.Storages.Where(x => x.tag == "Container" && x.Inventory.ItemsInInventory > 0).ToArray();
                worker.targetStorage = SetStorageTarget(worker.ItemsToGet.Items, possibleStorages);
            }
            else if (worker is WarehouseWorker)
            {
                StorageInteractable[] sortedStorages = worker.storageLocations.Storages.Where(x => x.tag == "Container" && x.Inventory.SpaceLeft > 0).OrderBy(y => y.Inventory.ItemsInInventory).ToArray();
                worker.targetStorage = SetStorageTarget(worker.Inventory.Items, sortedStorages);
            }
        }

        public void OnExit()
        {
        }
    }
}

