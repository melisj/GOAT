using Cinemachine;
using UnityEngine;

namespace Goat.CameraControls
{
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera vCam;
        [SerializeField] protected Vector2 minMaxZoom;
        [SerializeField] private int zoomStrength;
        //[SerializeField] private ZoomAxis zoomAxis;
        protected CinemachineComponentBase baseComp;

        protected virtual void Awake()
        {
            baseComp = vCam.GetCinemachineComponent(CinemachineCore.Stage.Body);
        }

        protected virtual float Zoom(float currentDist)
        {
            float cameraDistance = 0;
            //currentObject = currentTopViewMode == TopViewMode.thirdPerson ? thirdPersonObj.transform : pointToClickObj.transform;
            //Vector3 zoomVector = zoomAxis == ZoomAxis.x ? Vector3.right : zoomAxis == ZoomAxis.y ? Vector3.up : Vector3.forward;
            //  float posToCheck = zoomAxis == ZoomAxis.x ? currentTransform.localPosition.x : zoomAxis == ZoomAxis.y ? currentTransform.localPosition.y : currentTransform.localPosition.z;

            // Zoom in when we are scrolling up and aren't on the closest zoom level
            if (Input.mouseScrollDelta.y > 0 && currentDist > minMaxZoom.x)
            {
                cameraDistance += -zoomStrength * Time.unscaledDeltaTime;
            }

            // Zoom in when we are scrolling down and aren't on the farthest zoom level
            if (Input.mouseScrollDelta.y < 0 && currentDist < minMaxZoom.y)
            {
                cameraDistance += zoomStrength * Time.unscaledDeltaTime;
            }
            return cameraDistance;
        }
    }
}