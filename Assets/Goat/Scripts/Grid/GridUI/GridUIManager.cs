using UnityEngine;
using Sirenix.OdinInspector;
using Goat.Grid.Interactions;
using Goat.Events;
using System.Collections.Generic;

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

    // Manages the UI Elements to make certain that only one element is visible at a time
    public class GridUIManager : SerializedMonoBehaviour
    {
        [SerializeField] private Dictionary<GridUIElement, BasicGridUIElement> UIElements = new Dictionary<GridUIElement, BasicGridUIElement>();
        [SerializeField] private GridUIInfo gridUIInfo;
        private static BasicGridUIElement currentUIOpen;

        public void Awake()
        {
            InputManager.Instance.OnInputEvent += Instance_OnInputEvent;
            gridUIInfo.GridUIChangedEvent += GridUIInfo_GridUIChangedEvent;
        }

        public void OnDestroy() {
            InputManager.Instance.OnInputEvent -= Instance_OnInputEvent;
            gridUIInfo.GridUIChangedEvent -= GridUIInfo_GridUIChangedEvent;
        }

        private void GridUIInfo_GridUIChangedEvent(GridUIElement currentUI, GridUIElement prevUI) {
            ShowNewUI(currentUI);
        }

        private void Instance_OnInputEvent(KeyCode code, InputManager.KeyMode keyMode, InputMode inputMode)
        {
            if (code == KeyCode.V && keyMode == InputManager.KeyMode.Down)
            {
                gridUIInfo.CurrentUIElement = GridUIElement.Buying;
            }
        }

        // Disable, and enable a new element
        private void ShowNewUI(GridUIElement UIElement)
        {
            HideUI();
            if (UIElement != GridUIElement.None && UIElement != gridUIInfo.CurrentUIElement) {
                UIElements.TryGetValue(UIElement, out BasicGridUIElement element);
                currentUIOpen = element;
                currentUIOpen.ShowUI();
            }
        }

        // Hide the current element
        private void HideUI()
        {
            if (currentUIOpen != null)
                currentUIOpen.HideUI();
            currentUIOpen = null;
        }
    }
}