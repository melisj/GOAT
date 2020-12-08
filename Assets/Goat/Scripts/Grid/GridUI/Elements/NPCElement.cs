using Goat.AI;
using Goat.Storage;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Goat.Grid.Interactions.UI
{
    public class NPCElement : UISlotElement
    {
        [SerializeField] private TextMeshProUGUI totalPrice;
        [SerializeField] private TextMeshProUGUI customerName;
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
        private void EnableIcon(int iconIndex, Sprite newIcon, float price, int amount)
        {
            icons[iconIndex].SetIconData(newIcon, price, amount);
        }

        /// <summary>
        /// Sets the NPC element in the UI with the inventory of the given NPC
        /// </summary>
        /// <param name="args"> 0 = NPC </param>
        public override void SetUI(object[] args)
        {
            base.SetUI(args);
            if (args.Length != 1)
                return;

            NPC customer = (NPC)args[0];

            if (customer)
            {
                Dictionary<Resource, int> itemList = customer.inventory;
                // Add icons if pool is not enough
                while (itemList.Count > icons.Count)
                {
                    AddStorageIcon();
                }

                int i = 0;
                foreach (KeyValuePair<Resource, int> item in itemList)
                {
                    EnableIcon(i, item.Key.Image, item.Key.Price, item.Value);
                    i++;
                }

                // Disable the items that are not being used
                for(int j = itemList.Count; j < icons.Count; j++)
                {
                    DisableIcon(i);
                }
            }
        }
    }
}