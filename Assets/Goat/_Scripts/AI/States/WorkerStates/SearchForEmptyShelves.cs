using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI;
using Goat.Grid.Interactions;
using System.Linq;

namespace Goat.AI.States
{
    public class SearchForEmptyShelves : IState
    {
        StockClerk stockClerk;
        List<StorageInteractable> sortedStorages;

        public SearchForEmptyShelves(StockClerk stockClerk)
        {
            this.stockClerk = stockClerk;
        }

        public void Tick()
        {

        }

        private void FindStorageAndItems()
        {
            // Somehow check which shelves are already selected by other employeets
            StorageInteractable tempStorage;
            int itemsToGetCount = 0;
            while (itemsToGetCount < stockClerk.maxCarryLoad)
            {
                itemsToGetCount = 0;

                // Somehow check for each item to add if storage target is already in list and if item can still be added.

                //check how many items 
                foreach (var item in stockClerk.itemsToGet)
                {
                    itemsToGetCount += item.Value;
                }
            }
            // while itemsToGet.count < maxCarryLoad
            // take first empty storage from list and add to storagetargets list and remove from sortedStorages.
            // add items to itemsToGet until storages is finished or itemsToGet is full.
        }

        public void OnEnter()
        {
            sortedStorages = stockClerk.storageLocations.Storages.Where(x => x.tag == "Storage").OrderBy(y => y.GetItemCount).ToList();
        }

        public void OnExit()
        {
            sortedStorages.Clear();
        }
    }
}

