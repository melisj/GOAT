using Goat.Grid.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Goat.Grid.Interactions
{
    public class InteractableManager : MonoBehaviour
    {
        public delegate void InteractableClickEvent(Transform interactable);

        public static event InteractableClickEvent InteractableClickEvt;

        public delegate void SelectedInteractableChangeEvent(BaseInteractable interactable);

        public static event SelectedInteractableChangeEvent SelectedInteractableChangeEvt;

        [SerializeField] private LayerMask interactableMask;

        public const string StorageIconPrefabname = "ItemIcon";

        public const string ItemHolderName = "ItemHolder";
        public const string ItemHolderParentName = "ItemHolderParent";
        public const string ItemMaterialName = "VertexColorShader";

        public static Material ItemMaterial;

        public void Awake()
        {
            ItemMaterial = Resources.Load<Material>(ItemMaterialName);

            InputManager.Instance.OnInputEvent += Instance_OnInputEvent;
        }

        private void Instance_OnInputEvent(KeyCode code, InputManager.KeyMode keyMode, InputMode inputMode)
        {
            if (inputMode == InputMode.Select)
            {
                if (code == KeyCode.Mouse0 && keyMode.HasFlag(InputManager.KeyMode.Down))
                {
                    Debug.Log("");
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        if (!GridUIManager.Instance.IsElementSelected())
                            CheckForInteractable();
                        else
                            GridUIManager.Instance.HideUI();
                    }
                }
            }
        }

        public void CheckForInteractable()
        {
            if (InputManager.Instance.DoRaycastFromMouse(out RaycastHit hit, interactableMask))
            {
                if (hit.transform != null)
                    InteractableClickEvt?.Invoke(hit.transform);
            }
        }

        public static void ChangeSelectedInteractable(BaseInteractable interactable)
        {
            SelectedInteractableChangeEvt?.Invoke(interactable);
        }
    }
}