using Cinemachine;
using Goat.Grid.UI;
using UnityEngine;

namespace Goat.CameraControls
{
    public class CameraPanning : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera vCam;
        [SerializeField] private float speed;
        [SerializeField] private bool panWithinScreenOnly;
        private Transform currentObject;
        private Vector2 mousePos;

        [SerializeField] private GridUIInfo gridUIInfo;

        private void Update()
        {
            PanCamera(Time.unscaledDeltaTime * speed);
        }

        private void PanCamera(float mouseVelocity)
        {
            if (!currentObject)
            {
                currentObject = vCam.Follow;
            }
            if (gridUIInfo.IsUIActive) return;
            mousePos = Input.mousePosition;
            if (panWithinScreenOnly ?
                mousePos.x >= 0 && mousePos.x <= Screen.width &&
                mousePos.y >= 0 && mousePos.y <= Screen.height : true)
            {
                if ((mousePos.x >= Screen.width - 25))
                {
                    currentObject.position += new Vector3(mouseVelocity, 0.0f);
                }

                if ((mousePos.x <= 25))
                {
                    currentObject.position += new Vector3(-mouseVelocity, 0.0f);
                }

                if ((mousePos.y >= Screen.height - 25))
                {
                    currentObject.position += new Vector3(0.0f, 0.0f, mouseVelocity);
                }

                if ((mousePos.y <= 25))
                {
                    currentObject.position += new Vector3(0.0f, 0.0f, -mouseVelocity);
                }
            }
        }
    }
}