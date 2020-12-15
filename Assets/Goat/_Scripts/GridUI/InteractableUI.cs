using Goat.Grid.Interactions;
using Goat.Grid.Interactions.UI;
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
        Storage,
        NPC
    }

    /// <summary>
    /// Keeps track of the UI of the
    /// </summary>
    public class InteractableUI : BasicGridUIElement
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI infoText;
        [SerializeField] private Image interactableIcon;

        [SerializeField] private Transform UIElementSlot;
        [SerializeField] private Transform StockingUI;

        [SerializeField] private InteractablesInfo interactableInfo;
        [SerializeField] private InteractableUIElements elements;
        // Keeps track of all UI elements available
        private Dictionary<InteractableUIElement, UISlotElement> UIElements = new Dictionary<InteractableUIElement, UISlotElement>();
        private UISlotElement activeElement;
        private InteractableUIElement loadedType;

        private bool IsThisActive => gameObject.activeInHierarchy;

        protected virtual void Awake()
        {
            SpawnUIElements();
        }

        private void OnEnable()
        {
            interactableInfo.InteractableUpdateEvt += InteractableInfo_InteractableUpdateEvt;
        }

        private void OnDisable()
        {
            interactableInfo.InteractableUpdateEvt -= InteractableInfo_InteractableUpdateEvt;
        }

        private void InteractableInfo_InteractableUpdateEvt(BaseInteractable interactable)
        {
            InteractableUIElement elementToLoad = InteractableUIElement.None;

            if (interactable is StorageInteractable) elementToLoad = InteractableUIElement.Storage;
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

        // Set the default UI elements to the given params
        public void SetUI(string title,
            string description,
            InteractableUIElement elementToLoad,
            BaseInteractable info)
        {
            if (IsThisActive)
            {
                titleText.text = title;
                descriptionText.text = description;
                infoText.text = info.PrintObject(info);
            }
            LoadElement(elementToLoad, info.GetArgumentsForUI());
        }

        // Load a new UI element
        public void LoadElement(InteractableUIElement elementId, object[] args)
        {
            if (elementId == InteractableUIElement.None || loadedType != elementId)
                UnloadElement();

            if (IsThisActive && elementId != InteractableUIElement.None)
            {
                StockingUI.gameObject.SetActive(elementId == InteractableUIElement.Storage);
                loadedType = elementId;

                UIElements.TryGetValue(elementId, out UISlotElement element);
                activeElement = element;

                if (activeElement)
                {
                    activeElement.gameObject.SetActive(true);
                    SetElementValues(args);
                }
            }
        }

        // Unload the specific UI element
        public void UnloadElement()
        {
            if (IsThisActive && activeElement)
            {
                StockingUI.gameObject.SetActive(false);
                activeElement.gameObject.SetActive(false);
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