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

            // while itemsToGet.count < maxCarryLoad
            // take first empty storage from list and add to storagetargets list and remove from sortedStorages.
            // add items to itemsToGet until storages is finished or itemsToGet is full.
        }

        public void OnEnter()
        {
            sortedStorages = stockClerk.storageLocations.Storages.OrderBy(x => x.GetItemCount).ToList();
        }

        public void OnExit()
        {
            sortedStorages.Clear();
        }
    }
}

