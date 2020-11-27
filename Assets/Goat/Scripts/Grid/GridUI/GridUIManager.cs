using Goat.UI;
using UnityEngine;

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
    public class GridUIManager : MonoBehaviour
    {
        [SerializeField] private BasicGridUIElement buildingUI;
        [SerializeField] private BasicGridUIElement buyingUI;
        private static BasicGridUIElement currentUIOpen;

        public void Awake()
        {
            //BuildingUI = FindObjectOfType<BuildingUI>();
            //BuyingUI = FindObjectOfType<BuyingUI>();

            InputManager.Instance.OnInputEvent += Instance_OnInputEvent;
        }

        private void Instance_OnInputEvent(KeyCode code, InputManager.KeyMode keyMode, InputMode inputMode)
        {
            if (code == KeyCode.C && keyMode == InputManager.KeyMode.Down)
            {
                ShowNewUI(buildingUI);
            }
            if (code == KeyCode.V && keyMode == InputManager.KeyMode.Down)
            {
                ShowNewUI(buyingUI);
            }
        }

        // Disable, and enable a new element
        public static void ShowNewUI(BasicGridUIElement UIElement)
        {
            if (!IsSelectedSame(UIElement))
            {
                HideUI();
                currentUIOpen = UIElement;
                currentUIOpen.ShowUI();
            }
            else
                HideUI();
        }

        // Hide the current element
        public static void HideUI()
        {
            if (currentUIOpen != null)
                currentUIOpen.HideUI();
            currentUIOpen = null;
        }

        // Check if a element is selected
        public static bool IsElementSelected()
        {
            return currentUIOpen != null;
        }

        // Check if the given element is same as current selected
        public static bool IsSelectedSame(BasicGridUIElement UIElement)
        {
            if (currentUIOpen != null && !currentUIOpen.gameObject.activeInHierarchy)
                currentUIOpen = null;
            return currentUIOpen == UIElement;
        }
    }
}