using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.WSA.Input;

namespace GOAT.Grid
{
    public class InteractableManager : MonoBehaviour
    {
        public delegate void InteractableClickEvent(Transform interactable);
        public static event InteractableClickEvent InteractableClickEvt;

        public UI.InteractableUI interactableUI;

        public LayerMask mask;

        public static InteractableManager instance;

        public void Awake() {
            instance = this;
        }

        /// <summary>
        /// Raycast from the position of the mouse to the world
        /// </summary>
        /// <param name="hit"></param>
        /// <returns> Returns whether it hit something </returns>
        private bool DoRaycastFromMouse(out RaycastHit hit) {
            Vector3 mousePosition = Input.mousePosition + new Vector3(0, 0, Camera.main.nearClipPlane);
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3 cameraPerspective = mouseWorldPosition - Camera.main.transform.position;

            bool isHitting = Physics.Raycast(mouseWorldPosition, cameraPerspective, out RaycastHit mouseHit, Mathf.Infinity);
            hit = mouseHit;

            if (EventSystem.current.IsPointerOverGameObject())
                return false;
            return isHitting;
        }

        private void Update() {
            if (Input.GetMouseButtonDown(0)) {
                if (DoRaycastFromMouse(out RaycastHit hit)) {
                    if (hit.transform != null)
                        InteractableClickEvt?.Invoke(hit.transform);
                }
            }
        }
    }
}