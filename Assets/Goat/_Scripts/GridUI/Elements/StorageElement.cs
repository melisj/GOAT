using Goat.ScriptableObjects;
using Goat.Storage;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        [SerializeField] private InteractablesInfo info;

        private List<InventoryIcon> itemIcons = new List<InventoryIcon>();

        private int amountOfPrewarmedStorageElements = 10;

        [SerializeField] private bool showSeperateObject;

        [SerializeField] private bool hasMainResource;
        [SerializeField, ShowIf("hasMainResource")] private ResourceArray resourceArray;
        [SerializeField, ShowIf("hasMainResource")] private GameObject resourceListParent;

        /// <summary>
        /// 
        /// </summary>
        public override void InitUI() {
            base.InitUI();

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
        private void EnableIcon(int iconIndex, Sprite newIcon, int amount) {
            // Reset listeners
            itemIcons[iconIndex].IconButton.onClick.RemoveAllListeners();

            // Set the element active and set the icon
            itemIcons[iconIndex].gameObject.SetActive(true);
            itemIcons[iconIndex].SetIconData(newIcon, 0, amount);
        }

        #region Storage
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
                SpawnGroupedElements((Inventory)args[1]);
        }

        private void SpawnGroupedElements(Inventory inventory)
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
                EnableIcon(i, resource.Key.Image, resource.Value);
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
                    EnableIcon(total, inventory.Items.ElementAt(i).Key.Image, 0);

                    // Add the custom event to the resource
                    if (interactable is StorageInteractable)
                    {
                        // This needs to happen, otherwise the index will not be what it says it is
                        int index = i;
                        int totalIndex = total;
                        itemIcons[totalIndex].IconButton.onClick.AddListener(() => 
                            interactable.Remove(inventory.Items.ElementAt(index).Key, 1, out int removedAmount)
                            );
                    }
                }
            }
        }

        #endregion

        #region Main Resource UI

        private void SetupAllResources()
        {
            foreach(Resource resource in resourceArray.Resources)
            {
                if(resource.Available)
                {
                    //GameObject object = new GameObject();
                }
            } 
        }

        #endregion

        private void SetStorageLimitUI(string text) {
            itemsText.text = text;
        }
    }
}