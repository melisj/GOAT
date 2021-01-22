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
        private StockClerk stockClerk;
        public bool foundEmptyShelves { get; private set; }
        private float tickTime = 0, tickSpeed = 1;

        public SearchForEmptyShelves(StockClerk stockClerk)
        {
            this.stockClerk = stockClerk;
        }

        public void Tick()
        {
        }

        /// <summary>
        /// Gets a list of storage which need to be filled
        /// </summary>
        /// <param name="storages"> Returns a list of storage that the worker is planning to fill </param>
        private void FindStorageAndItems(List<StorageInteractable> storages)
        {
            if (storages.Count == 0 || storages == null) return;

            bool found = false;

            while (stockClerk.ItemsToGet.SpaceLeft > 0 && storages.Count > 0)
            {
                StorageInteractable tempStorage = storages.First();
                Resource tempResource = tempStorage.MainResource;
                int amountToGet = tempStorage.Inventory.SpaceLeft;

                stockClerk.ItemsToGet.Add(tempResource, amountToGet, out int amountStored);
                if (amountStored > 0)
                {
                    found = true;
                    tempStorage.selected = true;
                    stockClerk.TargetStorages.Add(tempStorage);
                }
                storages.RemoveAt(0);
            }

            foundEmptyShelves = found;
        }

        public void OnEnter()
        {
            foundEmptyShelves = false;
            tickTime = Time.time + (1 / tickSpeed);
            ClearCurrentShelves();

            // Return a list of possible storages to fill
            List<StorageInteractable> sortedStorages = stockClerk.Shelves.Storages.Where(x => x.MainResource != null && x.Inventory.SpaceLeft > 0 && !x.selected).OrderBy(y => y.Inventory.ItemsInInventory).ToList();
            // Return a list of storages to fill
            FindStorageAndItems(sortedStorages);
            sortedStorages.Clear();
        }

        private void ClearCurrentShelves()
        {
            if (stockClerk.TargetStorage != null)
                stockClerk.TargetStorage.selected = false;
            for (int i = 0; i < stockClerk.TargetStorages.Count; i++)
                stockClerk.TargetStorages[i].selected = false;

            stockClerk.TargetStorage = null;
            stockClerk.TargetStorages = new List<StorageInteractable>();
        }

        public void OnExit()
        {
        }
    }
}