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
        [SerializeField] private TextMeshProUGUI itemsText;
        [SerializeField] private Transform gridParent;

        [SerializeField] private PlayerInventory playerInventory;
        [SerializeField] private InteractablesInfo info;

        [SerializeField] private InteractableUI interactableUI;

        private List<InventoryIcon> itemIcons = new List<InventoryIcon>();

        private int amountOfPrewarmedStorageElements = 10;

        [SerializeField] private bool showSeperateObject;

        public override void InitUI() {
            base.InitUI();
            interactableUI = GetComponentInParent<InteractableUI>();

            for (int i = 0; i < amountOfPrewarmedStorageElements; i++) {
                AddStorageIcon();
            }
        }

        // Create a new storage icon
        private void AddStorageIcon() {
            GameObject instance = Instantiate(info.StorageIconPrefab, gridParent);
            instance.SetActive(false);
            itemIcons.Add(instance.GetComponent<InventoryIcon>());
        }

        // Disable a storage icon
        private void DisableIcon(int iconIndex) {
            itemIcons[iconIndex].gameObject.SetActive(false);
        }

        // Enable a storage icon with a new sprite
        private void EnableIcon(int iconIndex, Resource resource, int amount, Action callback) {
            // Reset listeners
            itemIcons[iconIndex].IconButton.onClick.RemoveAllListeners();

            // Set the element active and set the icon
            itemIcons[iconIndex].gameObject.SetActive(true);
            itemIcons[iconIndex].SetIconData(resource, 0, amount, callback);
        }

        /// <summary>
        /// Sets the storage UI up with the items given by the arguments
        /// </summary>
        /// <param name="args"> 0 = item text : 1 = inventory : 2 = StorageInteractable </param>
        public override void SetUI(object[] args) {
            base.SetUI(args);
            if (args.Length != 3)
                return;

            SetStorageLimitUI(args[0].ToString());

            if (showSeperateObject)
                SpawnSeperateElements((Inventory)args[1], (StorageInteractable)args[2]);
            else
                SpawnGroupedElements((Inventory)args[1], (StorageInteractable)args[2]);
        }

        private void SpawnGroupedElements(Inventory inventory, StorageInteractable interactable)
        {
            // Add icons if pool is not enough
            while (inventory.Items.Count > itemIcons.Count)
            {
                AddStorageIcon();
            }

            for(int i = inventory.Items.Count; i < itemIcons.Count; i++)
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

        private void SpawnSeperateElements(Inventory inventory, StorageInteractable interactable)
        {
            // Add icons if pool is not enough
            while (inventory.ItemsInInventory > itemIcons.Count)
            {
                AddStorageIcon();
            }


            for (int i = inventory.ItemsInInventory; i < itemIcons.Count; i++)
            {
                DisableIcon(i);
            }

            for (int i = 0, total = 0; i < inventory.Items.Count; i++)
            {
                for (int j = 0; j < inventory.Items.ElementAt(i).Value; j++, total++)
                {
                    Resource resource = inventory.Items.ElementAt(i).Key;

                    EnableIcon(total, inventory.Items.ElementAt(i).Key, 0, () => {
                        playerInventory.Inventory.Add(resource, 1, out int amountStored);
                        interactable.Inventory.Remove(resource, amountStored, out int removedAmount);
                    });
                }
            }
        }

        private void SetStorageLimitUI(string text) {
            itemsText.text = text;
        }
    }
}