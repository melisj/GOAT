﻿using Goat.Grid.Interactions;
using Goat.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Goat.Grid.UI;

namespace Goat.Player
{
    public class InventoryDisplayUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI storageText;
        [SerializeField] private Transform iconParent;
        [SerializeField] private GameObject stockingUI;

        [SerializeField] private InteractablesInfo info;
        [SerializeField] private PlayerInventory playerInventory;
        [SerializeField] private GridUIInfo gridUIInfo;

        [Serializable] private class SelectedResourceChanged : UnityEvent<Resource, Inventory, Inventory> { }
        [SerializeField] private SelectedResourceChanged selectedResourceChangeEvt;

        private List<InventoryIcon> itemIcons = new List<InventoryIcon>();

        private void OnEnable()
        {
            SetUI();
            playerInventory.Inventory.InventoryChangedEvent += Inventory_InventoryChangedEvent;
            gridUIInfo.GridUIChangedEvent += GridUIInfo_GridUIChangedEvent;
        }

        private void OnDisable()
        {
            playerInventory.Inventory.InventoryChangedEvent -= Inventory_InventoryChangedEvent;
            gridUIInfo.GridUIChangedEvent -= GridUIInfo_GridUIChangedEvent;
        }

        private void GridUIInfo_GridUIChangedEvent(GridUIElement currentUI, GridUIElement prevUI)
        {
            stockingUI.SetActive(false);
        }

        private void Inventory_InventoryChangedEvent(Resource resource, int amount, bool removed)
        {
            SetUI();
        }

        // Create a new storage icon
        private void AddStorageIcon()
        {
            GameObject instance = Instantiate(info.StorageIconPrefab, iconParent);
            instance.SetActive(false);
            itemIcons.Add(instance.GetComponent<InventoryIcon>());
        }

        // Disable a storage icon
        private void DisableIcon(int iconIndex)
        {
            itemIcons[iconIndex].gameObject.SetActive(false);
        }

        // Enable a storage icon with a new sprite
        private void EnableIcon(int iconIndex, Resource resource, int amount, Action callback)
        {
            // Reset listeners
            itemIcons[iconIndex].IconButton.onClick.RemoveAllListeners();

            // Set the element active and set the icon
            itemIcons[iconIndex].gameObject.SetActive(true);
            itemIcons[iconIndex].SetIconData(resource, 0, amount, callback);
        }

        public void SetUI()
        {
            storageText.text = string.Format("Player Inventory -=- {0} / {1}", playerInventory.Inventory.ItemsInInventory, playerInventory.Inventory.Capacity);
            SpawnGroupedElements(playerInventory.Inventory);
        }

        private void SpawnGroupedElements(Inventory inventory)
        {
            // Add icons if pool is not enough
            while (inventory.Items.Count > itemIcons.Count)
            {
                AddStorageIcon();
            }

            for (int i = inventory.Items.Count; i < itemIcons.Count; i++)
            {
                DisableIcon(i);
            }

            for (int i = 0; i < inventory.Items.Count; i++)
            {
                var resource = inventory.Items.ElementAt(i);
                EnableIcon(i, resource.Key, resource.Value, () => { 
                    selectedResourceChangeEvt?.Invoke(resource.Key, inventory, ((StorageInteractable)info.CurrentSelected).Inventory); 
                    stockingUI.SetActive(true);
                });
            }
        }
    }
}