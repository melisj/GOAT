using Goat.AI;
using Goat.Storage;
using Goat.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

namespace Goat.Grid.Interactions.UI
{
    public class NPCElement : UISlotElement
    {
        [SerializeField] private AnimateStorageElement animateElement;
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private TextMeshProUGUI totalPrice;
        [SerializeField] private TextMeshProUGUI customerName;
        [SerializeField] private RectTransform customerBorderLeft, customerBorderMiddle;
        [SerializeField] private Money money;
        [SerializeField] private IntEvent onSale;
        [SerializeField] private float margin;
        [SerializeField] private Button sellButton;
        [SerializeField] private Transform contentParent;

        private List<CellWithPriceAndAmount> uiCells = new List<CellWithPriceAndAmount>();

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

        public override void OpenUI()
        {
            animateElement.OpenStorage();
        }

        public override void CloseUI()
        {
            animateElement.CloseStorage();
        }

        // Create a new storage icon
        private void AddStorageIcon()
        {
            GameObject instance = Instantiate(cellPrefab, contentParent);
            instance.SetActive(false);
            uiCells.Add(instance.GetComponent<CellWithPriceAndAmount>());
        }

        // Disable a storage icon
        private void DisableIcon(int iconIndex)
        {
            uiCells[iconIndex].gameObject.SetActive(false);
        }

        // Enable a storage icon with a new sprite
        private void EnableIcon(int iconIndex, Resource resource, float price, int amount)
        {
            uiCells[iconIndex].gameObject.SetActive(true);
            uiCells[iconIndex].Setup(resource, (int)price, amount);
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
                while (amountItems > uiCells.Count)
                {
                    AddStorageIcon();
                }

                int i = 0;
                foreach (KeyValuePair<Resource, int> item in itemList)
                {
                    EnableIcon(i, item.Key, item.Key.Price(true), item.Value);
                    i++;
                }

                // Set button actions (leave store)
                sellButton.onClick.AddListener(() => Sell(customer, checkout));
            }

            // Disable the items that are not being used
            for (int i = amountItems; i < uiCells.Count; i++)
            {
                DisableIcon(i);
            }

            // Set the texts
            if (totalPrice)
                totalPrice.text = string.Format("Total price: {0}", customer ? customer.totalPriceProducts.ToString() : "-");
            if (customerName)
            {
                customerName.text = string.Format("Name: {0}", customer ? customer.ObjInstance.GetName : "No customer");
                ChangeIconWidth(customerName.text, customerName, customerBorderMiddle, customerBorderLeft);
            }
        }

        private void Sell(Customer customer, CheckoutInteractable checkout)
        {
            checkout.RemoveCustomerFromQueue();
            money.Amount += customer.totalPriceProducts;
            onSale.Raise((int)customer.totalPriceProducts);
        }

        protected void ChangeIconWidth(string change, TextMeshProUGUI textUI, RectTransform amountHolder, RectTransform leftBorder)
        {
            //InitialSize
            float initialSize = leftBorder.sizeDelta.x * 2;
            //  float iconWidth = (textUI.fontSize) + ((textUI.fontSize + margin) * (change.Length));
            float iconWidth = ((textUI.fontSize) + ((textUI.fontSize + margin) * (change.Length)));
            iconWidth -= initialSize;
            iconWidth = Mathf.Max(iconWidth, 1);
            amountHolder.sizeDelta = new Vector2(iconWidth, amountHolder.sizeDelta.y);
        }
    }
}