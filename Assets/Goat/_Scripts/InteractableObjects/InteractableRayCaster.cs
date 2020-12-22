using UnityEngine;
using UnityEngine.EventSystems;

namespace Goat.Grid.Interactions
{
    public class InteractableRayCaster : MonoBehaviour
    {
        public bool DoRaycastFromMouse(out RaycastHit hit, LayerMask mask)
        {
            Vector3 mousePosition = Input.mousePosition + new Vector3(0, 0, Camera.main.nearClipPlane);
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3 cameraPerspective = mouseWorldPosition - Camera.main.transform.position;

            bool isHitting = Physics.Raycast(mouseWorldPosition, cameraPerspective, out RaycastHit mouseHit, Mathf.Infinity, mask);
            hit = mouseHit;
            Debug.DrawRay(mouseWorldPosition, cameraPerspective * 100, isHitting ? Color.green : Color.red);
            if (EventSystem.current.IsPointerOverGameObject())
                return false;

            //if (isHitting)
            //{
            //    Debug.Log(hit.transform.parent.parent);
            //}
            return isHitting;
        }
    }
}