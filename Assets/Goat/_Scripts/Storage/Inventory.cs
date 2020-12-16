using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Storage
{
    [Serializable]
    public struct Inventory
    {
        public Dictionary<Resource, int> Items { get; private set; }
        private int itemsInInventory;
        private int capacity;

        public int SpaceLeft => capacity - itemsInInventory;
        public int ItemsInInventory => itemsInInventory;
        public int Capacity => capacity;

        public Inventory(int maxCapacity)
        {
            itemsInInventory = 0;
            this.capacity = maxCapacity;
            Items = new Dictionary<Resource, int>();
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
        }

        public bool Contains(Resource resource)
        {
            return Items.ContainsKey(resource);
        }

        public void SetInventory(Dictionary<Resource, int> newInventory)
        {
            Items = newInventory;
        }

        public void Clear()
        {
            Items.Clear();
            itemsInInventory = 0;
        }
    }
}