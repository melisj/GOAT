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
    public class GeneralUIManager : EventListenerKeyCodeModeEvent
    {
        [SerializeField] private Dictionary<GridUIElement, BasicGridUIElement> UIElements = new Dictionary<GridUIElement, BasicGridUIElement>();
        [SerializeField] private GridUIInfo gridUIInfo;
        private static BasicGridUIElement currentUIOpen;

        public void Awake()
        {
            gridUIInfo.GridUIChangedEvent += GridUIInfo_GridUIChangedEvent;
        }

        public void OnDestroy()
        {
            gridUIInfo.GridUIChangedEvent -= GridUIInfo_GridUIChangedEvent;
        }

        private void GridUIInfo_GridUIChangedEvent(GridUIElement currentUI, GridUIElement prevUI)
        {
            ShowNewUI(currentUI);
        }

        public override void OnEventRaised(KeyCodeMode value)
        {
            KeyCode code = KeyCode.None;
            KeyMode mode = KeyMode.None;

            value.Deconstruct(out code, out mode);

            OnInput(code, mode);
        }

        private void OnInput(KeyCode code, KeyMode keyMode)
        {
            if (code == KeyCode.V && keyMode == KeyMode.Down)
            {
                gridUIInfo.CurrentUIElement = GridUIElement.Buying;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                gridUIInfo.CurrentUIElement = GridUIElement.Buying;
            }
        }

        // Disable, and enable a new element
        private void ShowNewUI(GridUIElement UIElement)
        {
            HideUI();
            if (UIElement != GridUIElement.None && UIElement != gridUIInfo.CurrentUIElement)
            {
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