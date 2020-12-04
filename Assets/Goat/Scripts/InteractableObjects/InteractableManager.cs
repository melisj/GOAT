﻿using Goat.Grid.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Goat.Grid.Interactions
{
    public class InteractableManager : MonoBehaviour {
        public delegate void InteractableClickEvent(Transform interactable);
        public static event InteractableClickEvent InteractableClickEvt;

        [SerializeField] private LayerMask interactableMask;
        [SerializeField] private GridUIInfo gridUIInfo;

        public void Awake()
        {
            InputManager.Instance.OnInputEvent += Instance_OnInputEvent;
        }

        private void Instance_OnInputEvent(KeyCode code, InputManager.KeyMode keyMode, InputMode inputMode)
        {
            if (inputMode == InputMode.Select)
            {
                if (code == KeyCode.Mouse0 && keyMode.HasFlag(InputManager.KeyMode.Down))
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        if (!gridUIInfo.IsUIActive)
                            CheckForInteractable();
                        else
                            gridUIInfo.CurrentUIElement = GridUIElement.None;
                    }
                }
            }
        }

        public void CheckForInteractable()
        {
            if (InputManager.Instance.DoRaycastFromMouse(out RaycastHit hit, interactableMask))
            {
                if (hit.transform != null)
                {
                    InteractableClickEvt?.Invoke(hit.transform);
                }
            }
        }
    }
}