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


    public class InteractableUI : BasicGridUIElement
    {
        [Space(20)]

        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI infoText;
        [SerializeField] private Image interactableIcon;

        [SerializeField] private Transform UIElementSlot;

        private Dictionary<InteractableUIElement, UISlotElement> UIElements = new Dictionary<InteractableUIElement, UISlotElement>();
        private UISlotElement activeElement;

        public void Awake() {
            HideUI();

            int elementAmount = Enum.GetValues(typeof(InteractableUIElement)).Length;
            for (int i = 0; i < elementAmount; i++) {
                string uiElementName = ((InteractableUIElement)i).ToString();
                GameObject prefab = (GameObject)Resources.Load("InteractableUIElement-" + uiElementName);
                UISlotElement instance = Instantiate(prefab, UIElementSlot).GetComponent<UISlotElement>();
                instance.InitStorageUI( new object[]{ 10 } );

                UIElements.Add((InteractableUIElement)i, instance);
                instance.gameObject.SetActive(false);
            } 
        }

        public void SetUI(string title, string description, string info) {
            titleText.text = title;
            descriptionText.text = description;
            infoText.text = info;
        }

        public void LoadElement(InteractableUIElement elementId) {
            UIElements.TryGetValue(elementId, out UISlotElement element);
            UnloadElement();
            activeElement = element;
            activeElement.gameObject.SetActive(true);
        }

        public void UnloadElement() {
            activeElement?.gameObject.SetActive(false);
        }

        public void SetElementValue(object[] args) {
            activeElement.SetUI(args);
        } 

        // [TODO] For object icon a type of object needs to be send in a enum
    }
}