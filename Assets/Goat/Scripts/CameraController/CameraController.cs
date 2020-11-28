using Cinemachine;
using System;
using UnityEngine;

namespace Goat.CameraControls
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
        private CinemachineVirtualCamera thirdPersonCamera;
        [SerializeField] private CinemachineVirtualCamera topviewCamera;
        private CinemachineVirtualCamera stationaryCamera;
        [SerializeField] private Camera maincam;
        private Transform player;
        [SerializeField] private Transform panningObject;
        [SerializeField] private TopViewMode currentTopViewMode;
        [Header("Panning")]
        [SerializeField] private float speed;
        [SerializeField] private bool panWithinScreenOnly;
        [SerializeField] private float zoomAmount = 10f;
        private int minZoom = 5;
        [SerializeField] private int maxZoom;

        //[Header("3rd person turning")]
        private float rotSmoothTime = 0.2f;
        private float sensitivity = 4f;
        private Vector2 pitchMinMax = new Vector2(-40, 85);
        private float cameraMoveSpeed = 120f;

        private int currentZoom = 8;
        private CinemachineVirtualCamera currentActiveCamera;

        private Vector3 currentRotation;
        private Vector3 rotSmoothVel;
        private float yaw;
        private float pitch;
        private Vector2 mousePos;
        private bool isDragging;
        private Vector3 panOrigin;
        private Vector3 oldPanningPos;

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
            Cursor.lockState = CursorLockMode.None;
            InputManager.Instance.OnInputEvent += Instance_OnInputEvent;
        }

        private void Instance_OnInputEvent(KeyCode code, InputManager.KeyMode keyMode, InputMode inputMode)
        {
            ToggleMouse(code, keyMode);

            if (code == KeyCode.Home && keyMode == InputManager.KeyMode.Down)
            {
                panningObject.position = Vector3.zero;
                oldPanningPos = panningObject.position;
            }
        }

        private void Update()
        {
            mousePos = Input.mousePosition;
            MoveCamera();
            Zoom();
        }

        #endregion Unity Methods

        private void Zoom()
        {
            // Zoom in when we are scrolling up and aren't on the closest zoom level
            if (Input.mouseScrollDelta.y > 0 && currentZoom > minZoom)
            {
                panningObject.transform.position += Vector3.up * -zoomAmount;
                currentZoom--;
            }

            // Zoom in when we are scrolling down and aren't on the farthest zoom level
            if (Input.mouseScrollDelta.y < 0 && currentZoom < maxZoom)
            {
                panningObject.transform.position += Vector3.up * zoomAmount;
                currentZoom++;
            }
        }

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
            //if (Input.GetKeyDown(inputSettings.SwitchTopViewMode))
            //{
            //    SwitchTopViewMode();
            //}
            //if (Input.GetKeyDown(inputSettings.TopViewMode))
            //{
            //    SwitchCamera(topviewCamera);
            //}
            //if (Input.GetKeyDown(inputSettings.ThirdPersonMode))
            //{
            //    SwitchCamera(thirdPersonCamera);
            //}
            //if (Input.GetKeyDown(inputSettings.StationaryMode))
            //{
            //    SwitchCamera(stationaryCamera);
            //}
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

        private void MoveCamera(KeyCode code = KeyCode.None, Goat.InputManager.KeyMode keyMode = InputManager.KeyMode.None)
        {
            if (!topviewCamera.gameObject.activeInHierarchy || currentTopViewMode == TopViewMode.followPlayer) return;

            float mouseVelocity = Time.deltaTime * speed;
            // Only move the camera when the cursor is insize the window

            //  DragCamera(mouseVelocity, code, keyMode);
            if (panWithinScreenOnly ?
                mousePos.x >= 0 && mousePos.x <= Screen.width &&
                mousePos.y >= 0 && mousePos.y <= Screen.height : true)
            {
                PanCamera(mouseVelocity);
            }
        }

        private void DragCamera(float mouseVelocity)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                oldPanningPos = panningObject.position;
                panOrigin = maincam.ScreenToViewportPoint(mousePos);
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 screenPos = maincam.ScreenToViewportPoint(mousePos) - panOrigin;
                screenPos.z = screenPos.y;
                screenPos.y = 0;

                panningObject.position = oldPanningPos + -screenPos * speed;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
        }

        private void DragCamera(float mouseVelocity, KeyCode code, Goat.InputManager.KeyMode keyMode)
        {
            if (code == KeyCode.Mouse0 && keyMode.HasFlag(InputManager.KeyMode.Down))
            {
                isDragging = true;
                oldPanningPos = panningObject.position;
                panOrigin = maincam.ScreenToViewportPoint(mousePos);
            }

            if (code == KeyCode.Mouse0 && keyMode.HasFlag(InputManager.KeyMode.Pressed))
            {
                Vector3 screenPos = maincam.ScreenToViewportPoint(mousePos) - panOrigin;
                screenPos.z = screenPos.y;
                screenPos.y = 0;
                Debug.LogFormat("{0}+{1}+{2}", oldPanningPos, -screenPos, speed);
                panningObject.position = oldPanningPos + -screenPos * speed;
            }

            if (code == KeyCode.Mouse0 && keyMode.HasFlag(InputManager.KeyMode.Up))
            {
                isDragging = false;
            }
        }

        private void PanCamera(float mouseVelocity)
        {
            if (isDragging) return;
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
        }

        private void ToggleMouse(KeyCode code, Goat.InputManager.KeyMode keyMode)
        {
            if (keyMode == InputManager.KeyMode.Down)
            {
                if (code == (inputSettings.ToggleMouse) && Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
                else if (code == (inputSettings.ToggleMouse) && Cursor.lockState == CursorLockMode.None)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
    }
}