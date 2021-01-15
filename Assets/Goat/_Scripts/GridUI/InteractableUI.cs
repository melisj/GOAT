using Goat.Grid.Interactions;
using Goat.Grid.Interactions.UI;
using Goat.Player;
using System;
using System.Collections.Generic;
using TMPro;
using UnityAtoms;
using UnityEngine;
using UnityEngine.UI;

namespace Goat.Grid.UI
{
    public enum InteractableUIElement
    {
        None,
        ShelfStorage,
        CrateStorage,
        NPC
    }

    /// <summary>
    /// Keeps track of the UI of the
    /// </summary>
    public class InteractableUI : BasicGridUIElement
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private RectTransform headerBorderMiddle, headerBorderLeft;
        [SerializeField] private float margin;
        //[SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image interactableIcon;

        [SerializeField] private Transform UIElementSlot;
        [SerializeField] private StockingUI stockingUI;
        [SerializeField] private InventoryElement inventoryElement;
        [SerializeField] private UISlotElement[] slotElements;
        [SerializeField] private InteractablesInfo interactableInfo;
        [SerializeField] private InteractableUIElements elements;
        // Keeps track of all UI elements available
        private Dictionary<InteractableUIElement, UISlotElement> UIElements = new Dictionary<InteractableUIElement, UISlotElement>();
        private UISlotElement activeElement;
        private InteractableUIElement loadedType;

        private bool IsThisActive => gameObject.activeInHierarchy;
        public StockingUI StockingScript => stockingUI;

        protected virtual void Awake()
        {
            //SpawnUIElements();
            SetupUIElements(slotElements);
            if (!stockingUI)
                stockingUI = GetComponentInChildren<StockingUI>();
        }

        private void OnEnable()
        {
            interactableInfo.InteractableUpdateEvt += InteractableInfo_InteractableUpdateEvt;
        }

        private void OnDisable()
        {
            interactableInfo.InteractableUpdateEvt -= InteractableInfo_InteractableUpdateEvt;
        }

        public override void ShowUI()
        {
            base.ShowUI();
        }

        private void InteractableInfo_InteractableUpdateEvt(BaseInteractable interactable)
        {
            InteractableUIElement elementToLoad = InteractableUIElement.None;

            if (interactable is StorageInteractable) elementToLoad = ((StorageInteractable)interactable).ElementToLoad;
            else if (interactable is CheckoutInteractable) elementToLoad = InteractableUIElement.NPC;

            SetUI(interactable.Name, interactable.Description, elementToLoad, interactable);
        }

        // Create all the UI elements defined in the resources folder
        private void SpawnUIElements()
        {
            for (int i = 0, enumIndex = 1; i < elements.GetInteractableUIElements.Length; i++, enumIndex++)
            {
                GameObject prefab = elements.GetInteractableUIElements[i];

                if (prefab)
                {
                    UISlotElement instance = Instantiate(prefab, UIElementSlot).GetComponent<UISlotElement>();
                    instance.InitUI();

                    UIElements.Add((InteractableUIElement)enumIndex, instance);
                    instance.gameObject.SetActive(false);
                }
            }
        }

        private void SetupUIElements(UISlotElement[] slotElements)
        {
            for (int i = 0; i < slotElements.Length; i++)
            {
                UISlotElement slotElement = slotElements[i];
                slotElement.InitUI();

                UIElements.Add(slotElement.UiElementType, slotElement);
                //slotElement.gameObject.SetActive(false);
            }
        }

        // Set the default UI elements to the given params
        public void SetUI(string title,
            string description,
            InteractableUIElement elementToLoad,
            BaseInteractable info)
        {
            if (IsThisActive)
            {
                titleText.text = title;
                ChangeIconWidth(title, titleText, headerBorderMiddle, headerBorderLeft);
                //if (descriptionText)
                //    descriptionText.text = description;
            }
            LoadElement(elementToLoad, info.GetArgumentsForUI());
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

        // Load a new UI element
        public void LoadElement(InteractableUIElement elementId, object[] args)
        {
            if (elementId == InteractableUIElement.None || loadedType != elementId)
                UnloadElement();

            if (IsThisActive && elementId != InteractableUIElement.None)
            {
                //StockingScript.gameObject.SetActive(elementId == InteractableUIElement.ShelfStorage || elementId == InteractableUIElement.CrateStorage);
                loadedType = elementId;

                UIElements.TryGetValue(elementId, out UISlotElement element);
                activeElement = element;

                if (activeElement)
                {
                    //activeElement.gameObject.SetActive(true);
                    activeElement.OpenUI();
                    if (loadedType == InteractableUIElement.CrateStorage || loadedType == InteractableUIElement.ShelfStorage)
                        inventoryElement.SelectFirst();
                    SetElementValues(args);
                }
            }
        }

        // Unload the specific UI element
        public void UnloadElement()
        {
            if (IsThisActive && activeElement)
            {
                //StockingScript.gameObject.SetActive(false);
                //activeElement.gameObject.SetActive(false);
                activeElement.CloseUI();
                loadedType = InteractableUIElement.None;
            }
        }

        // Pass the arguments for the UI to the element currently in use
        private void SetElementValues(object[] args)
        {
            activeElement.SetUI(args);
        }
    }
}