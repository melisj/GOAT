using Goat.Storage;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GOAT.Grid.UI
{
    public enum InteractableUIElement 
    { 
        Storage
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

        // Keeps track of all UI elements available
        private Dictionary<InteractableUIElement, UISlotElement> UIElements = new Dictionary<InteractableUIElement, UISlotElement>();
        private UISlotElement activeElement;

        public void Awake() {
            HideUI();
            SpawnUIElements();
        }

        // Create all the UI elements defined in the resources folder
        private void SpawnUIElements() {
            int elementAmount = Enum.GetValues(typeof(InteractableUIElement)).Length;
            for (int i = 0; i < elementAmount; i++) {
                string uiElementName = ((InteractableUIElement)i).ToString();
                GameObject prefab = (GameObject)Resources.Load("InteractableUIElement-" + uiElementName);
                UISlotElement instance = Instantiate(prefab, UIElementSlot).GetComponent<UISlotElement>();
                instance.InitUI();

                UIElements.Add((InteractableUIElement)i, instance);
                instance.gameObject.SetActive(false);
            }
        }

        // Set the default UI elements to the given params
        public void SetUI(string title, string description, string info) {
            titleText.text = title;
            descriptionText.text = description;
            infoText.text = info;
        }

        // Load a new UI element
        public void LoadElement(InteractableUIElement elementId) {
            UIElements.TryGetValue(elementId, out UISlotElement element);
            UnloadElement();
            activeElement = element;
            activeElement.gameObject.SetActive(true);
        }

        // Unload the specific UI element
        public void UnloadElement() {
            activeElement?.gameObject.SetActive(false);
        }

        // Pass the arguments for the UI to the element currently in use
        public void SetElementValue(object[] args) {
            activeElement.SetUI(args);
        } 
    }
}