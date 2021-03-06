﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace Goat.Grid
{
    public class GridRayCaster : MonoBehaviour 
    {
        /// <summary>
        /// Raycast from the position of the mouse to the world
        /// </summary>
        /// <param name="hit"></param>
        /// <returns> Returns whether it hit something </returns>
        public bool DoRaycastFromMouse(out RaycastHit hit, LayerMask mask)
        {
            Vector3 mousePosition = Input.mousePosition + new Vector3(0, 0, Camera.main.nearClipPlane);
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3 cameraPerspective = mouseWorldPosition - Camera.main.transform.position;

            bool isHitting = DoRaycastFromPosition(mouseWorldPosition, cameraPerspective, out hit, mask);

            if (EventSystem.current.IsPointerOverGameObject())
                return false;

            return isHitting;
        }

        public bool DoRaycastFromPosition(Vector3 startPosition, Vector3 direction, out RaycastHit hit, LayerMask mask)
        {
            return Physics.Raycast(startPosition, direction, out hit, Mathf.Infinity, mask);
        }
    }
}