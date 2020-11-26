using Goat.Grid.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Grid.Interactions
{
    public class InteractableManager : MonoBehaviour
    {
        public delegate void InteractableClickEvent(Transform interactable);
        public static event InteractableClickEvent InteractableClickEvt;

        public delegate void SelectedInteractableChangeEvent(BaseInteractable interactable);
        public static event SelectedInteractableChangeEvent SelectedInteractableChangeEvt;

        [HideInInspector] public InteractableUI interactableUI;
        
        public const string StorageIconPrefabname = "ItemIcon";

        public const string ItemHolderName = "ItemHolder";
        public const string ItemHolderParentName = "ItemHolderParent";
        public const string ItemMaterialName = "VertexColorShader";


        public static InteractableManager instance;

        public void Awake() {
            instance = this;
            interactableUI = FindObjectOfType<InteractableUI>();
        }

        public void CheckForInteractable() {
            if (Grid.DoRaycastFromMouse(out RaycastHit hit)) {
                if (hit.transform != null)
                    InteractableClickEvt?.Invoke(hit.transform);
            }
        }

        public static void ChangeSelectedInteractable(BaseInteractable interactable) {
            SelectedInteractableChangeEvt?.Invoke(interactable);
        }

    }
}