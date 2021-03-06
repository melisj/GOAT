﻿using Goat.Grid.Interactions;
using Goat.Storage;
using TMPro;
using UnityEngine;
using Goat.Grid.UI;
using Goat.Grid.Interactions.UI;
using System.Linq;
using Sirenix.OdinInspector;

namespace Goat.Player
{
    public class InventoryElement : StorageElement
    {
        [Title("Inventory related")]
        [SerializeField] private GridUIInfo gridUIInfo;

        private void OnEnable()
        {
            SetUI();
            playerInventory.Inventory.InventoryChangedEvent += Inventory_InventoryChangedEvent;
            playerInventory.Inventory.InventoryResetEvent += SetUI;
            gridUIInfo.GridUIChangedEvent += GridUIInfo_GridUIChangedEvent;
        }

        public void SelectFirst()
        {
            /*if (uiCells.Count > 0 && uiCells[0] != null)
                uiCells[0].InvokeOnClick();*/
        }

        private void SetUI()
        {
            SetStorageLimitUI(string.Format("{0} / {1}", playerInventory.Inventory.ItemsInInventory, playerInventory.Inventory.Capacity));
            SpawnGroupedElements(playerInventory.Inventory);
        }

        protected void SpawnGroupedElements(Inventory inventory)
        {
            while (inventory.Items.Count > uiCells.Count)
            {
                AddStorageIcon();
            }

            for (int i = inventory.Items.Count; i < uiCells.Count; i++)
            {
                DisableIcon(i);
            }

            for (int i = 0; i < inventory.Items.Count; i++)
            {
                var resource = inventory.Items.ElementAt(i);
                EnableIcon(i, resource.Key, resource.Value, () =>
                {
                    if (info.CurrentSelected != null && info.CurrentSelected is StorageInteractable storage)
                    {
                        //interactableUI.StockingScript.ChangeResource(resource.Key, inventory, storage.Inventory);
                        //interactableUI.StockingScript.StockButtonText.text = "Stock item";

                        //interactableUI.StockingScript.StockingUIElement.gameObject.SetActive(true);
                        storage.Inventory.Add(resource.Key, GetAmountByClick(resource.Key, playerInventory.Inventory), out int amountStored);
                        playerInventory.Inventory.Remove(resource.Key, amountStored, out int removedAmount);
                    }
                });
            }
        }

        private void Inventory_InventoryChangedEvent(Resource resource, int amount, bool removed)
        {
            SetUI();
        }

        private void GridUIInfo_GridUIChangedEvent(UIElement currentUI, UIElement prevUI)
        {
            //interactableUI.StockingScript.StockingUIElement.SetActive(false);
        }

        private void OnDisable()
        {
            playerInventory.Inventory.InventoryChangedEvent -= Inventory_InventoryChangedEvent;
            gridUIInfo.GridUIChangedEvent -= GridUIInfo_GridUIChangedEvent;
            playerInventory.Inventory.InventoryResetEvent -= SetUI;
        }
    }
}