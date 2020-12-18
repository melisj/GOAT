using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI;
using Goat.Storage;
using Goat.Grid.Interactions;
using System.Linq;

namespace Goat.AI.States
{
    public class SearchForStorageInWarehouse : IState
    {
        Worker worker;
        public SearchForStorageInWarehouse(Worker worker)
        {
            this.worker = worker;
        }

        public void Tick()
        {

        }

        private StorageInteractable SetStorageTarget(Resource item, StorageInteractable[] storages)
        {
            if (storages.Length == 0 || storages == null || item == null)
            {
                Debug.LogError("No item or storages were found when calling Method: SetStorageTarget !");
                return null;
            }

            bool storageFound = false;
            
            for (int i = 0; i < storages.Length; i++)
            {
                if (storages[i].Inventory.Contains(item))
                {
                    storageFound = true;
                    return storages[i];
                }
            }

            if (!storageFound && worker is WarehouseWorker)
                return storages.First();
            else if (!storageFound && worker is StockClerk)
                Debug.LogWarningFormat("Item: {0} no longer in stock!", item.name);

            return null;
        }

        public void OnEnter()
        {
            if(worker is StockClerk)
            {
                StorageInteractable[] sortedStorages = worker.storageLocations.Storages.Where(x => x.tag == "Container").ToArray();
                worker.targetStorage = SetStorageTarget(worker.ItemsToGet.Items.Keys.First(), sortedStorages);
                sortedStorages = new StorageInteractable[0];
            }
            else if (worker is WarehouseWorker)
            {
                StorageInteractable[] sortedStorages = worker.storageLocations.Storages.Where(x => x.tag == "Container" && x.Inventory.SpaceLeft != 0).OrderBy(y => y.Inventory.ItemsInInventory).ToArray();
                worker.targetStorage = SetStorageTarget(worker.Inventory.Items.Keys.First(), sortedStorages);
                sortedStorages = new StorageInteractable[0];
            }
        }

        public void OnExit()
        {
        }
    }
}

