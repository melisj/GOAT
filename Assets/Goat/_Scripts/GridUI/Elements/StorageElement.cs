using Goat.Storage;
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

        /// <summary>
        /// Sets the storage UI up with the items given by the arguments
        /// </summary>
        /// <param name="args"> 0 = item text : 1 = items : 2 = StorageInteractable </param>
        public override void SetUI(object[] args) {
            base.SetUI(args);
            if (args.Length != 3)
                return;

            SetStorageLimitUI(args[0].ToString());

            if (showSeperateObject)
                SpawnSeperateElements((ItemInstance[])args[1], (StorageInteractable)args[2]);
            else
                SpawnGroupedElements((ItemInstance[])args[1]);
        }

        private void SpawnGroupedElements(ItemInstance[] itemArray)
        {
            List<Resource> resourceList = new List<Resource>();
            List<int> amountList = new List<int>();
            foreach (ItemInstance item in itemArray)
            {
                if (item != null)
                {
                    Resource resource = item.Resource;
                    if (!resourceList.Contains(resource))
                    {
                        resourceList.Add(resource);
                        amountList.Add(0);
                    }

                    amountList[resourceList.IndexOf(resource)] += 1;
                }
            }

            // Add icons if pool is not enough
            while (resourceList.Count > itemIcons.Count)
            {
                AddStorageIcon();
            }

            for(int i = resourceList.Count; i < itemIcons.Count; i++)
            {
                DisableIcon(i);
            }

            for (int i = 0; i < resourceList.Count; i++)
            {
                EnableIcon(i, resourceList[i].Image, amountList[i]);
            }
        }

        private void SpawnSeperateElements(ItemInstance[] itemList, StorageInteractable interactable)
        {
            // Add icons if pool is not enough
            while (itemList.Length > itemIcons.Count)
            {
                AddStorageIcon();
            }

            for (int i = 0; i < itemList.Length; i++)
            {
                // Disable the items that are not being used
                if (itemList[i] == null)
                {
                    DisableIcon(i);
                    continue;
                }
                else
                    EnableIcon(i, itemList[i].Resource.Image, 0);


                // Add the custom event to the resource
                if (interactable is StorageInteractable)
                {
                    // This needs to happen, otherwise the index will not be what it says it is
                    int index = i;
                    itemIcons[i].IconButton.onClick.AddListener(() => interactable.GetResource(index, true));
                }
            }
        }

        private void SetStorageLimitUI(string text) {
            itemsText.text = text;
        }
    }
}