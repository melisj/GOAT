using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Storage
{
    public struct Inventory
    {
        public Dictionary<Resource, int> InventoryInstance { get; }
        private int itemsInInventory;
        private int maxCapacity;

        private int spaceLeft => maxCapacity - itemsInInventory;

        public Inventory(int maxCapacity)
        {
            itemsInInventory = 0;
            this.maxCapacity = maxCapacity;
            InventoryInstance = new Dictionary<Resource, int>();
        }

        public void Add(Resource resource, int amount, out int amountStored)
        {
            amountStored = Mathf.Min(amount, spaceLeft);
            if (amountStored == 0) return;

            itemsInInventory += amountStored;

            if (InventoryInstance.ContainsKey(resource))
                InventoryInstance[resource] += amountStored;
            else
                InventoryInstance.Add(resource, amountStored);
        }

        public void Remove(Resource resource, int amount, out int amountRemoved)
        {
            if (InventoryInstance.ContainsKey(resource))
            {
                amountRemoved = Mathf.Min(amount, InventoryInstance[resource]);

                InventoryInstance[resource] -= amountRemoved;
                if (InventoryInstance[resource] <= 0)
                    InventoryInstance.Remove(resource);

                itemsInInventory -= amountRemoved;
            }
            else
                amountRemoved = 0;
        }



    }
}