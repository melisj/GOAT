using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAT.Grid
{
    public class InteractableManager : MonoBehaviour
    {
        public delegate void InteractableClickEvent(Transform interactable);
        public static event InteractableClickEvent InteractableClickEvt;

        [HideInInspector] public UI.InteractableUI interactableUI;

        public static InteractableManager instance;

        public void Awake() {
            instance = this;
            interactableUI = FindObjectOfType<UI.InteractableUI>();
        }

        public void CheckForInteractable() {
            if (Grid.DoRaycastFromMouse(out RaycastHit hit)) {
                if (hit.transform != null)
                    InteractableClickEvt?.Invoke(hit.transform);
            }
        }
    }
}