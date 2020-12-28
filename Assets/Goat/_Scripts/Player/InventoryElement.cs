using Goat.Grid.Interactions;
using Goat.Storage;
using TMPro;
using UnityEngine;
using Goat.Grid.UI;
using Goat.Grid.Interactions.UI;

namespace Goat.Player
{
    public class InventoryElement : StorageElement
    {
        [SerializeField] private GridUIInfo gridUIInfo;

        private void OnEnable()
        {
            SetUI();
            playerInventory.Inventory.InventoryChangedEvent += Inventory_InventoryChangedEvent;
            gridUIInfo.GridUIChangedEvent += GridUIInfo_GridUIChangedEvent;
        }

        private void SetUI()
        {
            itemsText.text = string.Format("{0} / {1}", playerInventory.Inventory.ItemsInInventory, playerInventory.Inventory.Capacity);
            SpawnGroupedElements(playerInventory.Inventory, ((StorageInteractable)info.CurrentSelected));
        }

        private void Inventory_InventoryChangedEvent(Resource resource, int amount, bool removed)
        {
            SetUI();
        }

        private void GridUIInfo_GridUIChangedEvent(UIElement currentUI, UIElement prevUI)
        {
            interactableUI.StockingScript.StockingUIElement.SetActive(false);
        }

        private void OnDisable()
        {
            playerInventory.Inventory.InventoryChangedEvent -= Inventory_InventoryChangedEvent;
            gridUIInfo.GridUIChangedEvent -= GridUIInfo_GridUIChangedEvent;
        }
    }
}