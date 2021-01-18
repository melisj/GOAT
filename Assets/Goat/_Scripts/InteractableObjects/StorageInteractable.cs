using Goat.Storage;
using Goat.Grid.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Goat.Pooling;
using System;

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
        [SerializeField, TabGroup("Storage")] protected int maxResources = 4;
        [SerializeField, TabGroup("Storage")] protected StorageEnviroment enviroment;
        [SerializeField, TabGroup("Storage")] protected InteractableUIElement elementToLoad = InteractableUIElement.ShelfStorage;

        public InteractableUIElement ElementToLoad => elementToLoad;

        private Resource mainResource;
        [SerializeField, TabGroup("References")] private MeshRenderer outlineRend;

        public Resource MainResource { get => mainResource; set => mainResource = value; }
        public bool selected { get; set; }

        protected override void Awake()
        {
            inventory = new Inventory(maxResources);
            base.Awake();
            Inventory.InventoryChangedEvent += Inventory_InventoryChangedEvent;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Inventory.InventoryChangedEvent -= Inventory_InventoryChangedEvent;
        }

        private void Inventory_InventoryChangedEvent(Resource resource, int amount, bool removed)
        {
            InvokeChange();
        }

        protected override void IsClicked(Transform clickedObj)
        {
            base.IsClicked(clickedObj);
        }

        public override object[] GetArgumentsForUI()
        {
            return new object[] {
            string.Format("{0} / {1}", inventory.ItemsInInventory, maxResources),
            inventory,
            this };
        }
    }
}