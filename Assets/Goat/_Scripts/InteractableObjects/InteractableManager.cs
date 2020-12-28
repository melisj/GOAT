using Goat.Grid.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Goat.Events;

namespace Goat.Grid.Interactions
{
    public class InteractableManager : EventListenerKeyCodeModeEvent
    {
        public delegate void InteractableClickEvent(Transform interactable);

        public static event InteractableClickEvent InteractableClickEvt;

        [SerializeField] private LayerMask interactableMask;
        [SerializeField] private InputModeVariable currentMode;
        [SerializeField] private InteractableRayCaster interactableRayCaster;
        [SerializeField] private GridUIInfo gridUIInfo;

        public override void OnEventRaised(KeyCodeMode value)
        {
            KeyCode code = KeyCode.None;
            KeyMode mode = KeyMode.None;

            value.Deconstruct(out code, out mode);
            OnInput(code, mode);
        }

        private void Update()
        {
            if (currentMode.InputMode == InputMode.Select)
            {
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    CheckForInteractable();
                    if (gridUIInfo.CurrentUIElement != UIElement.Interactable)
                    {
                        gridUIInfo.CurrentUIElement = UIElement.None;
                    }
                }
            }
        }

        private void OnInput(KeyCode code, KeyMode keyMode)
        {
            if (currentMode.InputMode == InputMode.Select)
            {
                if (code == KeyCode.Mouse0 && keyMode.HasFlag(KeyMode.Down))
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        //if (!gridUIInfo.IsUIActive)
                        CheckForInteractable();
                        if (gridUIInfo.CurrentUIElement != UIElement.Interactable)
                        {
                            gridUIInfo.CurrentUIElement = UIElement.None;
                        }
                    }
                }
            }
        }

        public void CheckForInteractable()
        {
            //if (interactableRayCaster.DoRaycastFromMouse(out RaycastHit hit, interactableMask))
            //{
            //    if (hit.transform != null)
            //    {
            //        InteractableClickEvt?.Invoke(hit.transform);
            //    }
            //}
            interactableRayCaster.DoRaycastFromMouse(out RaycastHit hit, interactableMask);

            InteractableClickEvt?.Invoke(hit.transform);
        }
    }
}