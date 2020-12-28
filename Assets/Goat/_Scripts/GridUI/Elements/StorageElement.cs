using Goat.Grid.UI;
using Goat.Player;
using Goat.Storage;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Goat.Grid.Interactions.UI
{
    /// <summary>
    /// UI element for storage objects
    /// </summary>
    public class StorageElement : UISlotElement
    {
        [SerializeField] protected GameObject cellPrefab;
        [SerializeField] protected TextMeshProUGUI itemsText;
        [SerializeField] protected Transform gridParent;
        [SerializeField] protected PlayerInventory playerInventory;
        [SerializeField] protected InteractablesInfo info;

        [SerializeField] protected InteractableUI interactableUI;

        private List<InventoryIcon> itemIcons = new List<InventoryIcon>();
        private List<CellWithInventoryAmount> uiCells = new List<CellWithInventoryAmount>();

        private int amountOfPrewarmedStorageElements = 10;

        [SerializeField] private bool showSeperateObject;

        public override void InitUI()
        {
            base.InitUI();
            interactableUI = GetComponentInParent<InteractableUI>();

            for (int i = 0; i < amountOfPrewarmedStorageElements; i++)
            {
                AddStorageIcon();
            }
        }

        // Create a new storage icon
        protected void AddStorageIcon()
        {
            GameObject instance = Instantiate(cellPrefab, gridParent);
            instance.SetActive(false);
            // itemIcons.Add(instance.GetComponent<InventoryIcon>());
            uiCells.Add(instance.GetComponent<CellWithInventoryAmount>());
        }

        // Disable a storage icon
        protected void DisableIcon(int iconIndex)
        {
            //  itemIcons[iconIndex].gameObject.SetActive(false);
            uiCells[iconIndex].gameObject.SetActive(false);
        }

        // Enable a storage icon with a new sprite
        protected void EnableIcon(int iconIndex, Resource resource, int amount, Action callback)
        {
            // Reset listeners
            //itemIcons[iconIndex].IconButton.onClick.RemoveAllListeners();
            uiCells[iconIndex].ResetOnClick();

            // Set the element active and set the icon
            // itemIcons[iconIndex].gameObject.SetActive(true);
            //itemIcons[iconIndex].SetIconData(resource, 0, amount, callback);

            uiCells[iconIndex].gameObject.SetActive(true);
            uiCells[iconIndex].Setup(resource, amount);
            uiCells[iconIndex].OnClick(callback);
        }

        /// <summary>
        /// Sets the storage UI up with the items given by the arguments
        /// </summary>
        /// <param name="args"> 0 = item text : 1 = inventory : 2 = StorageInteractable </param>
        public override void SetUI(object[] args)
        {
            base.SetUI(args);
            if (args.Length != 3)
                return;

            SetStorageLimitUI(args[0].ToString());

            if (showSeperateObject)
                SpawnSeperateElements((Inventory)args[1], (StorageInteractable)args[2]);
            else
                SpawnGroupedElements((Inventory)args[1], (StorageInteractable)args[2]);
        }

        protected void SpawnGroupedElements(Inventory inventory, StorageInteractable interactable)
        {
            // Add icons if pool is not enough
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
                    interactableUI.StockingScript.ChangeResource(resource.Key, interactable.Inventory, playerInventory.Inventory);
                    interactableUI.StockingScript.StockingUIElement.gameObject.SetActive(true);
                });
            }
        }

        protected void SpawnSeperateElements(Inventory inventory, StorageInteractable interactable)
        {
            // Add icons if pool is not enough
            while (inventory.ItemsInInventory > uiCells.Count)
            {
                AddStorageIcon();
            }

            for (int i = inventory.ItemsInInventory; i < uiCells.Count; i++)
            {
                DisableIcon(i);
            }

            for (int i = 0, total = 0; i < inventory.Items.Count; i++)
            {
                for (int j = 0; j < inventory.Items.ElementAt(i).Value; j++, total++)
                {
                    Resource resource = inventory.Items.ElementAt(i).Key;

                    EnableIcon(total, inventory.Items.ElementAt(i).Key, 0, () =>
                    {
                        playerInventory.Inventory.Add(resource, 1, out int amountStored);
                        interactable.Inventory.Remove(resource, amountStored, out int removedAmount);
                    });
                }
            }
        }

        protected void SetStorageLimitUI(string text)
        {
            itemsText.text = text;
        }
    }
}