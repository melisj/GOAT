using UnityEngine;

namespace GOAT.Grid.UI
{
    // Basic element for grid UI elements
    // Is used to manage showing and hiding the UI
    public class BasicGridUIElement : MonoBehaviour
    {
        [SerializeField] private GameObject PanelToHide;

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
        [HideInInspector] public SelectionModeUI selectionModeUI;
        private static BasicGridUIElement currentUIOpen;

        public void Awake() {
            tileEditUI = FindObjectOfType<TileEditUI>();
            selectionModeUI = FindObjectOfType<SelectionModeUI>();
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
