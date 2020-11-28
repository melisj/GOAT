using Goat.UI;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Goat.Grid.Interactions;

namespace Goat.Grid.UI
{
    // Basic element for grid UI elements
    // Is used to manage showing and hiding the UI
    public class BasicGridUIElement : MonoBehaviour
    {
        [SerializeField] private GameObject PanelToHide;

        public virtual void ShowUI()
        {
            PanelToHide.SetActive(true);
        }

        public virtual void HideUI()
        {
            PanelToHide.SetActive(false);
        }
    }

    public enum GridUIElement
    {
        building,
        buying,
        interactable
    }

    // Manages the UI Elements to make certain that only one element is visible at a time
    public class GridUIManager : SerializedMonoBehaviour
    {
        [SerializeField] private Dictionary<GridUIElement, BasicGridUIElement> UIElements = new Dictionary<GridUIElement, BasicGridUIElement>();
        private static BasicGridUIElement currentUIOpen;

        private static GridUIManager instance;

        public static GridUIManager Instance
        {
            get
            {
                if (!instance) {
                    instance = FindObjectOfType<GridUIManager>();
                }
                return instance;
            }
        }

        public void Awake()
        {
            InputManager.Instance.OnInputEvent += Instance_OnInputEvent;
        }

        private void Instance_OnInputEvent(KeyCode code, InputManager.KeyMode keyMode, InputMode inputMode)
        {
            if (code == KeyCode.C && keyMode == InputManager.KeyMode.Down)
            {
                ShowNewUI(GridUIElement.building);
            }
            if (code == KeyCode.V && keyMode == InputManager.KeyMode.Down)
            {
                ShowNewUI(GridUIElement.buying);
            }
        }

        // Disable, and enable a new element
        public void ShowNewUI(GridUIElement UIElement)
        {
            UIElements.TryGetValue(UIElement, out BasicGridUIElement element);
            if (!IsSelectedSame(element))
            {
                HideUI();
                currentUIOpen = element;
                currentUIOpen.ShowUI();
            }
            else
                HideUI();
        }

        // Hide the current element
        public void HideUI()
        {
            if (currentUIOpen != null)
                currentUIOpen.HideUI();
            currentUIOpen = null;
        }

        // Check if a element is selected
        public bool IsElementSelected()
        {
            return currentUIOpen != null;
        }

        // Check if the given element is same as current selected
        public bool IsSelectedSame(BasicGridUIElement UIElement)
        {
            if (currentUIOpen != null && !currentUIOpen.gameObject.activeInHierarchy)
                currentUIOpen = null;
            return currentUIOpen == UIElement;
        }
        
        public void SetInteractableUI(string title,
            string description,
            InteractableUIElement elementToLoad,
            BaseInteractable info,
            object[] args) {
            if (currentUIOpen is InteractableUI)
                ((InteractableUI)currentUIOpen).SetUI(title, description, elementToLoad, info, args);
        }
    }
}