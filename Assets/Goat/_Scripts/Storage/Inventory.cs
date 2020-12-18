using Newtonsoft.Json;
using Sirenix.Utilities;
using System;
using System.Collections;
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

        public delegate void InventoryChanged();
        public event InventoryChanged InventoryChangedEvent;

        public delegate void InventoryReset();
        public event InventoryReset InventoryResetEvent;

        public Inventory(int maxCapacity)
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

            InventoryChangedEvent?.Invoke();
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

            InventoryChangedEvent?.Invoke();
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

        public void Load(string storageString, ref GridObjectsList objectList)
        {
            Dictionary<int, int> jsonItems = JsonConvert.DeserializeObject<Dictionary<int, int>>(storageString);
            Dictionary<Resource, int> items = new Dictionary<Resource, int>();
            foreach (var item in jsonItems)
            {
                items.Add((Resource)objectList.GetObject(item.Key), item.Value);
            }

            SetInventory(items);
        }

        public void Clear()
        {
            Items.Clear();
            itemsInInventory = 0;

            InventoryResetEvent?.Invoke();
        }
    }
}