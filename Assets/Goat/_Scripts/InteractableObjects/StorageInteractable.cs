using Goat.Storage;
using Goat.Grid.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Goat.Pooling;

namespace Goat.Grid.Interactions
{
    /// <summary>
    /// Storage object placed on the grid
    /// Contains info about the resources it holds
    /// Contains info about its enviroment conditions
    /// </summary>
    public class StorageInteractable : BaseInteractable
    {
        [Header("Storage")]
        private ItemInstance[] itemList;
        [SerializeField]
        public ItemInstance[] ItemList
        {
            get => itemList;
            set
            {
                // Load in a new save
                itemList = value;
                InvokeChange();
            }
        }

        public List<ItemInstance> GetItems { get => ItemList.ToList(); }
        [SerializeField] protected int maxResources = 4;
        [SerializeField] protected StorageEnviroment enviroment;

        // Properties
        public int GetItemCount { get => itemList.Count(x => x != null); }
        public int SpaceLeft { get => Mathf.Abs(GetItemCount - maxResources); }

        protected override void Awake()
        {
            base.Awake();
        }

        public override object[] GetArgumentsForUI() {
            return new object[] {
            string.Format("Storage -=- {0}/{1}", GetItemCount, maxResources),
            itemList,
            this };
        }

        #region Inventory Management

        /// <summary>
        /// Add already instatiated instances of items
        /// </summary>
        /// <param name="items"> List of items being stored (removes the ones it is able to store)</param>
        /// <returns> Returns true if an item has been stored, false if none are stored </returns>
        public virtual bool AddResource(ref List<ItemInstance> items) {
            int amountWantingToBeStored = items.Count();
            int amountBeingStored = Mathf.Min(SpaceLeft, amountWantingToBeStored);

            // Check if there is space left
            if (amountBeingStored == 0)
                return false;

            // Store items in the list
            for (int i = amountBeingStored - 1; i >= 0; i--) {
                itemList[itemList.ToList().FindIndex(x => x == null)] = items[i];

                items.RemoveAt(i);
            }
       
            InvokeChange();

            return true;
        }

        /// <summary>
        /// Add resources based on amount and type of resource
        /// </summary>
        /// <param name="type"> Give type it should generate </param>
        /// <param name="amount"> Give amount it should generate </param>
        /// <param name="storedAmount"> Outs the amount that was actually stored </param>
        /// <returns> Return whether it stored at least one item </returns>
        public virtual bool AddResource(Resource type, int amount, out int storedAmount) {
            int amountBeingStored = Mathf.Min(SpaceLeft, amount);
            List<ItemInstance> items = new List<ItemInstance>();

            if (amount != amountBeingStored)
                Debug.LogWarningFormat("Could not add {0} of type {1} to inventory!", amount - amountBeingStored, type.name);

            // Store items in the list
            for (int i = 0; i < amount; i++) {
                items.Add(new ItemInstance(type));
            }

            storedAmount = amountBeingStored;
            return AddResource(ref items);
        }

        /// <summary>
        /// Get an item with given index
        /// Removes item from the storage and returns it
        /// </summary>
        /// <param name="index"> Index of the item you want </param>
        /// <param name="returnToStock"> Return the item to the stock by adding to the resources </param>
        /// <returns> Returns the selected item </returns>
        public virtual ItemInstance GetResource(int index, bool returnToStock = true) {
            ItemInstance item = itemList[index];
            itemList[index] = null;

            if(returnToStock)
                item.Resource.Amount++;

            InvokeChange();
            return item;
        }

        /// <summary>
        /// Returns all items from the storage
        /// Remove all items from the shelf
        /// </summary>
        /// <param name="index"> Index of the item you want </param>
        /// <param name="returnToStock"> Return the item to the stock by adding to the resources </param>
        /// <returns> Returns all the stored items </returns>
        public virtual List<ItemInstance> GetAllResources(bool returnToStock = true) {
            List<ItemInstance> oldItemList = itemList.ToList();

            if (returnToStock) {
                foreach (ItemInstance item in this.itemList) {
                    item.Resource.Amount++;
                }
            }
            ResetStorage();

            return oldItemList;
        }

        protected virtual void ResetStorage() {
            itemList = new ItemInstance[maxResources];
            InvokeChange();
        }

        #endregion

        public override string PrintObject(object obj)
        {
            return base.PrintObject(this);
        }
    }
}