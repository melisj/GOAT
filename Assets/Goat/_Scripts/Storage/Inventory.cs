using Goat.Saving;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Goat.Storage
{
    [Serializable]
    public class Inventory
    {
        [SerializeField, ReadOnly] private Dictionary<Resource, int> items;
        public Dictionary<Resource, int> Items { get => items; private set => items = value; }
        [SerializeField, ReadOnly] private int itemsInInventory;
        [SerializeField, ReadOnly] private int capacity;

        public int SpaceLeft => capacity - itemsInInventory;
        public int ItemsInInventory => itemsInInventory;
        public int Capacity => capacity;

        public bool InfiniCapacity { get; set; }

        public delegate void InventoryChanged(Resource resource, int amount, bool removed);

        public event InventoryChanged InventoryChangedEvent;

        public delegate void InventoryReset();

        public event InventoryReset InventoryResetEvent;

        [HideInInspector] public event EventHandler InventoryAddedEvent, InventoryRemovedEvent;

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

            if (!InfiniCapacity)
            {
                itemsInInventory += amountStored;

                if (Items.ContainsKey(resource))
                    Items[resource] += amountStored;
                else
                    Items.Add(resource, amountStored);
            }

            // [Warning] Might cause issues when amount is not the same as amount that is actually added
            OnInventoryChanged(amount);
            // ---

            InventoryChangedEvent?.Invoke(resource, amountStored, false);
        }

        public void Remove(Resource resource, int amount, out int amountRemoved)
        {
            if (Items.ContainsKey(resource))
            {
                amountRemoved = Mathf.Min(amount, Items[resource]);

                if (!InfiniCapacity)
                {
                    Items[resource] -= amountRemoved;
                    if (Items[resource] <= 0)
                        Items.Remove(resource);

                    itemsInInventory -= amountRemoved;
                }
            }
            else
                amountRemoved = 0;

            // [Warning] Might cause issues when amount is not the same as amount that is actually removed
            OnInventoryChanged(-amount);
            // ---

            InventoryChangedEvent?.Invoke(resource, amountRemoved, true);
        }

        public bool Contains(Resource resource)
        {
            return Items.ContainsKey(resource);
        }

        public void SetInventory(Dictionary<Resource, int> newInventory)
        {
            Items = newInventory;
            foreach (var item in Items)
            {
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

            if (!InfiniCapacity)
            {
                Items.Clear();
                itemsInInventory = 0;
                Dictionary<int, int> jsonItems = JsonConvert.DeserializeObject<Dictionary<int, int>>(storageString);
                foreach (var item in jsonItems)
                {
                    int amountToFill = Mathf.Min(SpaceLeft, item.Value);

                    if (amountToFill == 0) { Debug.LogWarning("No more room left in inventory, aborting rest of loading inventory!"); break; }

                    Items.Add((Resource)objectList.GetObject(item.Key), amountToFill);
                    itemsInInventory += amountToFill;
                }

                InventoryResetEvent?.Invoke();
            }
        }

        public void Clear()
        {
            Items.Clear();
            itemsInInventory = 0;

            InventoryResetEvent?.Invoke();
        }

        public void OnInventoryChanged(int changedAmount)
        {
            if (changedAmount == 0)
                return;
            else if (changedAmount < 0)
                InventoryRemovedEvent?.Invoke(this, null);
            else if (changedAmount > 0)
                InventoryAddedEvent?.Invoke(this, null);
        }
    }
}