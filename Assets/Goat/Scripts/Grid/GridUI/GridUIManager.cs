using UnityEngine;

namespace Goat.Grid.UI
{
    // Basic element for grid UI elements
    // Is used to manage showing and hiding the UI
    public class BasicGridUIElement : MonoBehaviour
    {
        [SerializeField] private GameObject PanelToHide;
        [HideInInspector] public Grid grid;

        private void Awake()
        {
            grid = FindObjectOfType<Grid>();
        }

        public virtual void ShowUI() {
            PanelToHide.SetActive(true);
        }

        public virtual void HideUI() {
            PanelToHide.SetActive(false);
        }
    }

    // Manages the UI Elements to make certain that only one element is visible at a time
    public class GridUIManager : MonoBehaviour
    {
        [HideInInspector] public TileEditUI tileEditUI;
        [HideInInspector] public EditModeUI editModeUI;
        [HideInInspector] public InteractableUI interactableUI;
        private static BasicGridUIElement currentUIOpen;

        public void Awake() {
            tileEditUI = FindObjectOfType<TileEditUI>();
            interactableUI = FindObjectOfType<InteractableUI>();
            editModeUI = FindObjectOfType<EditModeUI>();
        }

        // Disable, and enable a new element
        public static void ShowNewUI(BasicGridUIElement UIElement) {
            if(!IsSelectedSame(UIElement)) {
                HideUI();
                currentUIOpen = UIElement;
                currentUIOpen.ShowUI();
            }
        }

        // Hide the current element
        public static void HideUI() {
            if (currentUIOpen != null)
                currentUIOpen.HideUI();
            currentUIOpen = null; 
        }

        // Check if a element is selected
        public static bool IsElementSelected() {
            return currentUIOpen != null;
        }

        // Check if the given element is same as current selected
        public static bool IsSelectedSame(BasicGridUIElement UIElement) {
            return currentUIOpen == UIElement;
        }
    }
}
