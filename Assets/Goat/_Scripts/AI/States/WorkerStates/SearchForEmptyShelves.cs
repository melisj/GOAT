using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI;
using Goat.Grid.Interactions;
using System.Linq;
using Goat.Storage;

namespace Goat.AI.States
{
    public class SearchForEmptyShelves : IState
    {
        StockClerk stockClerk;
        private bool foundEmptyShelves;

        public SearchForEmptyShelves(StockClerk stockClerk)
        {
            this.stockClerk = stockClerk;
        }

        public void Tick()
        {
        }

        private void FindStorageAndItems(List<StorageInteractable> storages)
        {
            if (storages.Count == 0 || storages == null) return;

            // Somehow check which shelves are already selected by other employeets
            while (stockClerk.ItemsToGet.SpaceLeft > 0 && storages.Count > 0)
            {
                StorageInteractable tempStorage = storages.First();
                Resource tempResource = tempStorage.MainResource;
                int amountToGet = tempStorage.Inventory.SpaceLeft;

                stockClerk.ItemsToGet.Add(tempResource, amountToGet, out int amountStored);
                if (amountStored > 0)
                {
                    stockClerk.targetStorages.Add(tempStorage);
                    storages.RemoveAt(0);
                }
            }

            foundEmptyShelves = true;
        }

        public void OnEnter()
        {
            foundEmptyShelves = false;
            List<StorageInteractable> sortedStorages = stockClerk.storageLocations.Storages.Where(x => x.tag == "Storage" && x.MainResource != null && x.Inventory.SpaceLeft != 0).OrderBy(y => y.Inventory.ItemsInInventory).ToList();
            FindStorageAndItems(sortedStorages);
            sortedStorages.Clear();
        }

        public void OnExit()
        {
        }
    }
}

