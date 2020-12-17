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
        private Inventory inventory;
        public Inventory Inventory => inventory;

        [Header("Storage")]
        [SerializeField] protected int maxResources = 4;
        [SerializeField] protected StorageEnviroment enviroment;

        protected override void Awake()
        {
            inventory = new Inventory(maxResources);
            base.Awake();
            Inventory.InventoryChangedEvent += Inventory_InventoryChangedEvent;
        }

        protected void OnDestroy()
        {
            Inventory.InventoryChangedEvent -= Inventory_InventoryChangedEvent;
        }

        private void Inventory_InventoryChangedEvent()
        {
            InvokeChange();
        }

        public override object[] GetArgumentsForUI() {
            return new object[] {
            string.Format("Storage -=- {0}/{1}", inventory.ItemsInInventory, maxResources),
            inventory,
            this };
        }

        #region Inventory Management

        /// <summary>
        /// Add resources based on amount and type of resource
        /// </summary>
        /// <param name="resource"> Give type it should generate </param>
        /// <param name="amount"> Give amount it should generate </param>
        /// <param name="storedAmount"> Outs the amount that was actually stored </param>
        /// <returns> Return whether it stored at least one item </returns>
        public virtual bool Add(Resource resource, int amount, out int storedAmount) {
            inventory.Add(resource, amount, out storedAmount);
            return storedAmount != 0;
        }

        /// <summary>
        /// Get an item with given index
        /// Removes item from the storage and returns it
        /// </summary>
        /// <param name="index"> Index of the item you want </param>
        /// <param name="returnToStock"> Return the item to the stock by adding to the resources </param>
        /// <returns> Returns the selected item </returns>
        public virtual void Remove(Resource resource, int amount, out int removedAmount) {
            inventory.Remove(resource, amount, out removedAmount);
        }

        #endregion
    }
}