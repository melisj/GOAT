using Goat.AI;
using Goat.Storage;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Goat.Grid.Interactions.UI
{
    public class NPCElement : UISlotElement
    {
        [SerializeField] private TextMeshProUGUI totalPrice;
        [SerializeField] private TextMeshProUGUI customerName;
        [SerializeField] private Button sellButton;
        [SerializeField] private Transform contentParent;

        private List<InventoryIcon> icons = new List<InventoryIcon>();

        [SerializeField] private InteractablesInfo info;

        private int amountPrewarmedInventoryElements = 10;

        public override void InitUI()
        {
            base.InitUI();

            for (int i = 0; i < amountPrewarmedInventoryElements; i++)
            {
                AddStorageIcon();
            }
        }

        // Create a new storage icon
        private void AddStorageIcon()
        {
            GameObject instance = Instantiate(info.InventoryIconPrefab, contentParent);
            instance.SetActive(false);
            icons.Add(instance.GetComponent<InventoryIcon>());
        }

        // Disable a storage icon
        private void DisableIcon(int iconIndex)
        {
            icons[iconIndex].gameObject.SetActive(false);
        }

        // Enable a storage icon with a new sprite
        private void EnableIcon(int iconIndex, Resource resource, float price, int amount)
        {
            icons[iconIndex].gameObject.SetActive(true);
            icons[iconIndex].SetIconData(resource, price, amount);
        }

        /// <summary>
        /// Sets the NPC element in the UI with the inventory of the given NPC
        /// </summary>
        /// <param name="args"> 0 = NPC : 1 = InteractableObject </param>
        public override void SetUI(object[] args)
        {
            base.SetUI(args);
            if (args.Length != 2)
                return;

            Customer customer = (Customer)args[0];
            CheckoutInteractable checkout = (CheckoutInteractable)args[1];

            // Reset variables
            sellButton.onClick.RemoveAllListeners();

            int amountItems = 0;
            if (customer)
            {
                Dictionary<Resource, int> itemList = customer.Inventory.Items;
                amountItems = itemList.Count;

                // Add icons if pool is not enough
                while (amountItems > icons.Count)
                {
                    AddStorageIcon();
                }

                int i = 0;
                foreach (KeyValuePair<Resource, int> item in itemList)
                {
                    EnableIcon(i, item.Key, item.Key.Price, item.Value);
                    i++;
                }

                // Set button actions (leave store)
                sellButton.onClick.AddListener(checkout.RemoveCustomerFromQueue);
            }

            // Disable the items that are not being used
            for (int i = amountItems; i < icons.Count; i++)
            {
                DisableIcon(i);
            }

            // Set the texts
            if (totalPrice)
                totalPrice.text = string.Format("Total price: {0}", customer ? customer.totalPriceProducts.ToString() : "-");
            if (customerName)
                customerName.text = string.Format("Name: {0}", customer ? customer.name : "-");
        }
    }
}