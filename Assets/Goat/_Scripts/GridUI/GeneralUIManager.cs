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
        [SerializeField] private bool disableCanvasInstead;
        [SerializeField, HideIf("disableCanvasInstead")] private GameObject PanelToHide;
        [SerializeField, ShowIf("disableCanvasInstead")] private Canvas canvasToDisable;
        [SerializeField] private UIElement type;
        public UIElement Type => type;

        public virtual void ShowUI()
        {
            if (disableCanvasInstead)
                canvasToDisable.enabled = true;
            else
                PanelToHide.SetActive(true);
        }

        public virtual void HideUI()
        {
            if (disableCanvasInstead)
                canvasToDisable.enabled = false;
            else
                PanelToHide.SetActive(false);
        }
    }

    // Manages the UI Elements to make certain that only one element is visible at a time
    public class GeneralUIManager : EventListenerKeyCodeModeEvent
    {
        [SerializeField] private Dictionary<UIElement, BasicGridUIElement> UIElements = new Dictionary<UIElement, BasicGridUIElement>();
        [SerializeField] private GridUIInfo gridUIInfo;
        private static BasicGridUIElement currentUIOpen;

        [Button]
        private void SetupDictionary()
        {
            UIElements.Clear();
            BasicGridUIElement[] basicGridUIElements = GetComponentsInChildren<BasicGridUIElement>();
            for (int i = 0; i < basicGridUIElements.Length; i++)
            {
                BasicGridUIElement element = basicGridUIElements[i];
                if (UIElements.ContainsKey(element.Type))
                {
                    Debug.LogError($"Type {element.Type} already added to dictionary, click on me for the object", element.gameObject);
                    continue;
                }

                UIElements.Add(element.Type, element);
            }
        }

        public void Awake()
        {
            gridUIInfo.GridUIChangedEvent += GridUIInfo_GridUIChangedEvent;
        }

        public void OnDestroy()
        {
            gridUIInfo.GridUIChangedEvent -= GridUIInfo_GridUIChangedEvent;
        }

        private void GridUIInfo_GridUIChangedEvent(UIElement currentUI, UIElement prevUI)
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
                gridUIInfo.CurrentUIElement = UIElement.Buying;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                gridUIInfo.CurrentUIElement = UIElement.Buying;
            }
        }

        // Disable, and enable a new element
        private void ShowNewUI(UIElement UIElement)
        {
            HideUI();
            if (UIElement != UIElement.None)// && UIElement != gridUIInfo.CurrentUIElement
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