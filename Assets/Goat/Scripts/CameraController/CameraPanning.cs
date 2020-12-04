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
            if (GridUIManager.IsUIActive) return;
            mousePos = Input.mousePosition;
            if (panWithinScreenOnly ?
                mousePos.x >= 0 && mousePos.x <= Screen.width &&
                mousePos.y >= 0 && mousePos.y <= Screen.height : true)
            {
                Vector3 moveVector = Vector3.zero;
                if ((mousePos.x >= Screen.width - 25))
                {
                    moveVector = new Vector3(mouseVelocity, 0.0f);
                    //   moveVector = transform.right * mouseVelocity;
                    currentObject.position += moveVector;
                }

                if ((mousePos.x <= 25))
                {
                    moveVector = new Vector3(-mouseVelocity, 0.0f);
                    //      moveVector = transform.right * -mouseVelocity;

                    currentObject.position += moveVector;
                }

                if ((mousePos.y >= Screen.height - 25))
                {
                    moveVector = new Vector3(0.0f, 0.0f, mouseVelocity);
                    //   moveVector = transform.forward * mouseVelocity;
                    currentObject.position += moveVector;
                }

                if ((mousePos.y <= 25))
                {
                    moveVector = new Vector3(0.0f, 0.0f, -mouseVelocity);
                    //  moveVector = transform.forward * -mouseVelocity;

                    currentObject.position += moveVector;
                }
            }
        }
    }
}