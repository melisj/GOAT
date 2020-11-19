using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GOAT.Grid
{
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

    public class GridUIManager : MonoBehaviour
    {
        private static BasicGridUIElement currentUIOpen;

        public static void ShowNewUI(BasicGridUIElement UIElement) {
            if(!IsSelectedSame(UIElement)) {
                HideUI(currentUIOpen);
                currentUIOpen = UIElement;
                currentUIOpen.ShowUI();
            }
        }

        public static void HideUI(BasicGridUIElement UIElement) {
            if (UIElement != null)
                UIElement.HideUI();
            currentUIOpen = null; 
        }

        public static void HideUI() {
            HideUI(currentUIOpen);
        }

        public static bool IsElementSelected() {
            return currentUIOpen != null;
        }

        public static bool IsSelectedSame(BasicGridUIElement UIElement) {
            return currentUIOpen == UIElement;
        }
    }
}
