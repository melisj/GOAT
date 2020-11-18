using Cinemachine;
using UnityEngine;

namespace Goat.Camera
{
    public class CameraController : MonoBehaviour
    {
        public enum TopViewMode
        {
            followPlayer = 0,
            panning = 1
        }

        [Header("Input settings")]
        [SerializeField] private PlayerInputSettings inputSettings;

        [Header("Camera")]
        [SerializeField] private CinemachineVirtualCamera thirdPersonCamera;
        [SerializeField] private CinemachineVirtualCamera topviewCamera;
        [SerializeField] private CinemachineVirtualCamera stationaryCamera;
        [SerializeField] private Transform player;
        [SerializeField] private Transform panningObject;
        [SerializeField] private TopViewMode currentTopViewMode;
        [Header("Zoom")]
        [SerializeField] private float speed = 40f;
        [SerializeField] private float zoomAmount = 10f;
        [SerializeField] private int maxZoom = 10;
        [SerializeField] private int minZoom = 0;
        [SerializeField] private int minZoom3rdPerson = 0;
        [SerializeField] private int maxZoom3rdPerson = 10;

        [Header("3rd person turning")]
        [SerializeField] private float rotSmoothTime = 0.2f;
        [SerializeField] private float sensitivity = 4f;
        [SerializeField] private Vector2 pitchMinMax = new Vector2(-40, 85);
        [SerializeField] private float cameraMoveSpeed = 120f;

        private int currentZoom = 8;
        private CinemachineVirtualCamera currentActiveCamera;

        private Vector3 currentRotation;
        private Vector3 rotSmoothVel;
        private float yaw;
        private float pitch;
        private Vector2 mousePos;

        public bool ThirdPersonActive => currentActiveCamera == thirdPersonCamera;
        /// <summary>
        /// Returns euler rotation based on mouse positions
        /// </summary>
        public Vector3 GetLookEuler()
        {
            Vector3 euler = Vector3.zero;
            Vector2 axis = GetInputAxis();

            yaw += axis.x * sensitivity;
            pitch -= axis.y * sensitivity;

            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotSmoothVel, rotSmoothTime);
            euler = currentRotation;

            return euler;
        }

        #region Unity Methods

        private void Awake()
        {
            currentActiveCamera = topviewCamera;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            CheckInput();
        }

        private void FixedUpdate()
        {
            MoveCamera();
        }

        #endregion Unity Methods

        private Vector2 GetInputAxis()
        {
            Vector2 mouseAxis = Vector2.zero;
            mouseAxis.x = Input.GetAxis("Mouse X");
            mouseAxis.y = Input.GetAxis("Mouse Y");

            return mouseAxis;
        }

        private void CheckInput()
        {
            mousePos = Input.mousePosition;

            if (Input.GetKeyDown(inputSettings.SwitchTopViewMode))
            {
                SwitchTopViewMode();
            }
            if (Input.GetKeyDown(inputSettings.TopViewMode))
            {
                SwitchCamera(topviewCamera);
            }
            if (Input.GetKeyDown(inputSettings.ThirdPersonMode))
            {
                SwitchCamera(thirdPersonCamera);
            }
            if (Input.GetKeyDown(inputSettings.StationaryMode))
            {
                SwitchCamera(stationaryCamera);
            }

            Zoom();

            ToggleMouse();
        }

        private void SwitchCamera(CinemachineVirtualCamera nextCam)
        {
            currentActiveCamera.gameObject.SetActive(false);
            nextCam.gameObject.SetActive(true);
            currentActiveCamera = nextCam;
        }

        private void SwitchTopViewMode()
        {
            switch (currentTopViewMode)
            {
                case TopViewMode.panning:
                    currentTopViewMode = TopViewMode.followPlayer;
                    topviewCamera.Follow = player;
                    break;

                case TopViewMode.followPlayer:
                    currentTopViewMode = TopViewMode.panning;
                    Cursor.lockState = CursorLockMode.None;
                    topviewCamera.Follow = panningObject;
                    break;
            }
        }

        private void Zoom()
        {
            if (currentActiveCamera == thirdPersonCamera)
            {
                ZoomThirdPerson();
                return;
            }

            Transform cameraTrans = currentActiveCamera.transform;

            // Zoom in when we are scrolling up and aren't on the closest zoom level
            if (Input.mouseScrollDelta.y > 0 && currentZoom > minZoom)
            {
                cameraTrans.position += Vector3.up * zoomAmount;
                currentZoom--;
            }

            // Zoom in when we are scrolling down and aren't on the farthest zoom level
            if (Input.mouseScrollDelta.y < 0 && currentZoom < maxZoom)
            {
                cameraTrans.position += Vector3.up * -zoomAmount;
                currentZoom++;
            }
        }

        private void ZoomThirdPerson()
        {
            // Zoom in when we are scrolling up and aren't on the closest zoom level
            if (Input.mouseScrollDelta.y > 0 && currentZoom > minZoom3rdPerson)
            {
            ///    thirdPersonBody.CameraDistance += zoomAmount;
                currentZoom--;
            }

            // Zoom in when we are scrolling down and aren't on the farthest zoom level
            if (Input.mouseScrollDelta.y < 0 && currentZoom < maxZoom3rdPerson)
            {
               // thirdPersonBody.CameraDistance -= zoomAmount;
                currentZoom++;
            }
        }

        private void MoveCamera()
        {
            if (!topviewCamera.gameObject.activeInHierarchy || currentTopViewMode == TopViewMode.followPlayer) return;

            float mouseVelocity = Time.deltaTime * speed;
            // Only move the camera when the cursor is insize the window
            //if (mousePos.x >= 0 && mousePos.x <= Screen.width &&
            //    mousePos.y >= 0 && mousePos.y <= Screen.height)
            //{
            if ((mousePos.x >= Screen.width - 25))
            {
                panningObject.position += new Vector3(mouseVelocity, 0.0f);
            }

            if ((mousePos.x <= 25))
            {
                panningObject.position += new Vector3(-mouseVelocity, 0.0f);
            }

            if ((mousePos.y >= Screen.height - 25))
            {
                panningObject.position += new Vector3(0.0f, 0.0f, mouseVelocity);
            }

            if ((mousePos.y <= 25))
            {
                panningObject.position += new Vector3(0.0f, 0.0f, -mouseVelocity);
            }
            //  }
        }

        private void ToggleMouse()
        {
            if (Input.GetKeyDown(inputSettings.ToggleMouse) && Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else if (Input.GetKeyDown(inputSettings.ToggleMouse) && Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}