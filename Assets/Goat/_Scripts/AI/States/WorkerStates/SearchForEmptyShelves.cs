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
        public bool foundEmptyShelves { get; private set; }
        private float tickTime = 0, tickSpeed = 1;

        public SearchForEmptyShelves(StockClerk stockClerk)
        {
            this.stockClerk = stockClerk;
        }

        public void Tick()
        {
            //if(tickTime <= Time.time)
            //{
            //    tickTime = Time.time + (1 / tickSpeed);
            //    List<StorageInteractable> sortedStorages = stockClerk.storageLocations.Storages.Where(x => x.tag == "Storage" && x.MainResource != null && x.Inventory.SpaceLeft > 0 && !x.selected).OrderBy(y => y.Inventory.ItemsInInventory).ToList();
            //    FindStorageAndItems(sortedStorages);
            //    sortedStorages.Clear();
            //}
        }

        private void FindStorageAndItems(List<StorageInteractable> storages)
        {
            if (storages.Count == 0 || storages == null) return;

            bool found = false;
            // Somehow check which shelves are already selected by other employeets
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
                    stockClerk.targetStorages.Add(tempStorage);                    
                }
                storages.RemoveAt(0);
            }

            foundEmptyShelves = found;
        }

        public void OnEnter()
        {
            Debug.Log("StockClerk started searching for empty shelves");
            foundEmptyShelves = false;
            tickTime = Time.time + (1/ tickSpeed);
            ClearCurrentShelves();

            List<StorageInteractable> sortedStorages = stockClerk.storageLocations.Storages.Where(x => x.tag == "Storage" && x.MainResource != null && x.Inventory.SpaceLeft > 0 && !x.selected).OrderBy(y => y.Inventory.ItemsInInventory).ToList();
            FindStorageAndItems(sortedStorages);
            sortedStorages.Clear();

        }

        private void ClearCurrentShelves()
        {
            if(stockClerk.targetStorage != null)
                stockClerk.targetStorage.selected = false;
            for (int i = 0; i < stockClerk.targetStorages.Count; i++)
                stockClerk.targetStorages[i].selected = false;


            stockClerk.targetStorage = null;
            stockClerk.targetStorages = new List<StorageInteractable>();
        }

        public void OnExit()
        {
        }
    }
}

