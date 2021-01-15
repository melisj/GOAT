using Cinemachine;
using UnityEngine;

namespace Goat.CameraControls
{
    public class CameraRotate : RotateWithMouse
    {
        [SerializeField] private Camera maincam;
        [SerializeField] private CinemachineVirtualCamera topviewCamera;

        private Vector3 panOrigin;

        protected override void Rotate()
        {
            Vector3 msPos = GetInputAxis();
            if (Input.GetMouseButtonDown(1))
            {
                panOrigin = maincam.ScreenToViewportPoint(msPos);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (Input.GetMouseButton(1))
            {
                Vector3 screenPos = maincam.ScreenToViewportPoint(msPos) - panOrigin;
                screenPos.z = 0;

                Vector3 switchedPos = screenPos;
                switchedPos.x = screenPos.y;
                switchedPos.y = screenPos.x;

                topviewCamera.transform.eulerAngles = GetLookEuler();
            }

            if (Input.GetMouseButtonUp(1))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}