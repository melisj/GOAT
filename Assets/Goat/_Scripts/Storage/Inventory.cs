using Goat.Saving;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Storage
{
    [Serializable]
    public class Inventory
    {
        public Dictionary<Resource, int> Items { get; private set; }
        private int itemsInInventory;
        private int capacity;

        public int SpaceLeft => capacity - itemsInInventory;
        public int ItemsInInventory => itemsInInventory;
        public int Capacity => capacity;

        public delegate void InventoryChanged(Resource resource, int amount, bool removed);
        public event InventoryChanged InventoryChangedEvent;

        public delegate void InventoryReset();
        public event InventoryReset InventoryResetEvent;

        public Inventory(int maxCapacity, GridObjectsList obj = null)
        {
            itemsInInventory = 0;
            this.capacity = maxCapacity;
            Items = new Dictionary<Resource, int>();

            InventoryResetEvent?.Invoke();
        }

        public void Add(Resource resource, int amount, out int amountStored)
        {
            amountStored = Mathf.Min(amount, SpaceLeft);
            if (amountStored == 0) return;

            itemsInInventory += amountStored;

            if (Items.ContainsKey(resource))
                Items[resource] += amountStored;
            else
                Items.Add(resource, amountStored);

            InventoryChangedEvent?.Invoke(resource, amountStored, false);
        }

        public void Remove(Resource resource, int amount, out int amountRemoved)
        {
            if (Items.ContainsKey(resource))
            {
                amountRemoved = Mathf.Min(amount, Items[resource]);

                Items[resource] -= amountRemoved;
                if (Items[resource] <= 0)
                    Items.Remove(resource);

                itemsInInventory -= amountRemoved;
            }
            else
                amountRemoved = 0;

            InventoryChangedEvent?.Invoke(resource, amountRemoved, true);
        }

        public bool Contains(Resource resource)
        {
            return Items.ContainsKey(resource);
        }

        public void SetInventory(Dictionary<Resource, int> newInventory)
        {
            Items = newInventory;
            foreach (var item in Items) {
                itemsInInventory += item.Value;
            }

            InventoryResetEvent?.Invoke();
        }

        public string Save()
        {
            Dictionary<int, int> tempStorage = new Dictionary<int, int>();
            foreach (var item in Items)
            {
                tempStorage.Add(item.Key.ID, item.Value);
            }
            return JsonConvert.SerializeObject(tempStorage);
        }

        public void Load(string storageString, GridObjectsList objectList)
        {
            if (objectList == null) { Debug.LogError("Inventory could not be loaded, the GridObjectList was not found."); return; }

            Items.Clear();
            itemsInInventory = 0;
            Dictionary<int, int> jsonItems = JsonConvert.DeserializeObject<Dictionary<int, int>>(storageString);
            foreach (var item in jsonItems)
            {
                Items.Add((Resource)objectList.GetObject(item.Key), item.Value);
                itemsInInventory += item.Value;
            }

            InventoryResetEvent?.Invoke();
        }

        public void Clear()
        {
            Items.Clear();
            itemsInInventory = 0;

            InventoryResetEvent?.Invoke();
        }
    }
}