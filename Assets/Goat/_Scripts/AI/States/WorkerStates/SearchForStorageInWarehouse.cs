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
    /// <summary>
    /// Find storages in warehouse to put items into
    /// </summary>
    public class SearchForStorageInWarehouse : IState
    {
        private Worker worker;
        public bool nothingFound;

        public SearchForStorageInWarehouse(Worker worker)
        {
            this.worker = worker;
        }

        public void Tick()
        {
        }

        /// <summary>
        /// Returns storage interactable to move to
        /// </summary>
        /// <param name="items"> Items to place in storage or remove from storage </param>
        /// <param name="storages"> Possible storage to move to </param>
        /// <returns></returns>
        private StorageInteractable SetStorageTarget(Dictionary<Resource, int> items, StorageInteractable[] storages)
        {
            // Nothing found or to take/place
            if (storages.Length == 0 || storages == null || items == null || items.Count == 0)
            {
                if (worker is StockClerk)
                {
                    worker.ItemsToGet.Clear();
                }

                nothingFound = true;
                return null;
            }

            bool storageFound = false;

            int index = 0;
            // Get a storage with matching items in inventory
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

            // If no storage was found return the first for warehouseworker
            if (!storageFound && worker is WarehouseWorker)
                return storages.First();
            // Else let stockclerk know there is no storage to move to
            else if (!storageFound && worker is StockClerk)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    Debug.LogWarningFormat("Item: {0} no longer in stock!", items.ElementAt(i).Key.name);
                }
                worker.ItemsToGet.Clear();
                nothingFound = true;
            }

            return null;
        }

        public void OnEnter()
        {
            nothingFound = false;
            worker.TargetStorage = null;
            if (worker is StockClerk)
            {
                StorageInteractable[] possibleStorages = worker.WarehouseStorages.Storages.Where(x => x.Inventory.ItemsInInventory > 0).ToArray();
                worker.TargetStorage = SetStorageTarget(worker.ItemsToGet.Items, possibleStorages);
            }
            else if (worker is WarehouseWorker)
            {
                StorageInteractable[] sortedStorages = worker.WarehouseStorages.Storages.Where(x => x.Inventory.SpaceLeft > 0).OrderBy(y => y.Inventory.ItemsInInventory).ToArray();
                worker.TargetStorage = SetStorageTarget(worker.Inventory.Items, sortedStorages);
            }
        }

        public void OnExit()
        {
        }
    }
}