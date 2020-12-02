using Cinemachine;
using Goat.Grid.UI;
using System;
using UnityEngine;

namespace Goat.CameraControls
{
    public class CameraController : MonoBehaviour
    {
        public enum TopViewMode
        {
            thirdPerson = 0,
            clickToMove = 1
        }

        [Header("Input settings")]
        [SerializeField] private PlayerInputSettings inputSettings;
        [SerializeField] private float zoomStrength = 10f;
        [Header("Camera")]
        [SerializeField] private Camera maincam;
        [SerializeField] private CinemachineVirtualCamera thirdPersonCamera;
        [SerializeField] private CinemachineVirtualCamera topviewCamera;
        private CinemachineVirtualCamera stationaryCamera;
        [Header("TopView")]
        [SerializeField] private Vector2 minMaxZoomTopView;
        [SerializeField] private TopViewMode currentTopViewMode;
        [SerializeField] private GameObject pointToClickObj;
        [Header("Panning")]
        [SerializeField] private float speed;
        [SerializeField] private bool panWithinScreenOnly;
        [Header("3rdPerson")]
        [SerializeField] private Vector2 minMaxZoom3rdPerson;
        [SerializeField] private GameObject thirdPersonObj;

        private Transform currentObject;
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
            currentActiveCamera = thirdPersonCamera;
            currentObject = currentTopViewMode == TopViewMode.thirdPerson ? thirdPersonObj.transform : pointToClickObj.transform;

            Cursor.lockState = CursorLockMode.None;
            InputManager.Instance.OnInputEvent += Instance_OnInputEvent;
            InputManager.Instance.InputModeChanged += Instance_InputModeChanged;
        }

        private void Instance_InputModeChanged(object sender, InputMode e)
        {
            if (e != InputMode.Select)
            {
                SwitchToTopView();
            }
        }

        private void Instance_OnInputEvent(KeyCode code, InputManager.KeyMode keyMode, InputMode inputMode)
        {
            ToggleMouse(code, keyMode);

            if (code == KeyCode.Home && keyMode == InputManager.KeyMode.Down)
            {
                currentObject.position = Vector3.zero;
                oldPanningPos = currentObject.position;
            }
            if (code == KeyCode.Alpha3 && keyMode == InputManager.KeyMode.Down)
            {
                SwitchTopViewMode();
                InputManager.Instance.InputMode = InputMode.Select;
            }
            if (code == KeyCode.Alpha4 && keyMode == InputManager.KeyMode.Down)
            {
                GoToPlayer();
            }
        }

        private void GoToPlayer()
        {
            Vector3 newPos = thirdPersonObj.transform.position;
            newPos.y = pointToClickObj.transform.position.y;
            pointToClickObj.transform.position = newPos;
        }

        private void Update()
        {
            mousePos = Input.mousePosition;
            MoveCamera();
            RotateCamera();
            Zoom();
        }

        private void RotateCamera()
        {
            if (currentTopViewMode == TopViewMode.thirdPerson)
                thirdPersonObj.transform.eulerAngles = GetLookEuler();
        }

        #endregion Unity Methods

        private void Zoom()
        {
            currentObject = currentTopViewMode == TopViewMode.thirdPerson ? thirdPersonObj.transform : pointToClickObj.transform;
            Vector3 zoomVector = currentTopViewMode == TopViewMode.thirdPerson ? Vector3.forward : Vector3.up;
            Vector2 minMaxZoom = currentTopViewMode == TopViewMode.thirdPerson ? minMaxZoom3rdPerson : minMaxZoomTopView;
            // Zoom in when we are scrolling up and aren't on the closest zoom level
            if (Input.mouseScrollDelta.y > 0 && currentObject.transform.position.y > minMaxZoom.x)
            {
                currentObject.transform.position += Vector3.up * -zoomStrength * Time.unscaledDeltaTime;
            }

            // Zoom in when we are scrolling down and aren't on the farthest zoom level
            if (Input.mouseScrollDelta.y < 0 && currentObject.transform.position.y < minMaxZoom.y)
            {
                currentObject.transform.position += Vector3.up * zoomStrength * Time.unscaledDeltaTime;
            }
        }

        private Vector2 GetInputAxis()
        {
            Vector2 mouseAxis = Vector2.zero;
            mouseAxis.x = Input.GetAxis("Mouse X");
            mouseAxis.y = Input.GetAxis("Mouse Y");

            return mouseAxis;
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
                case TopViewMode.clickToMove:
                    SwitchTo3rdPerson();
                    break;

                case TopViewMode.thirdPerson:
                    SwitchToTopView();
                    break;
            }
        }

        private void SwitchTo3rdPerson()
        {
            currentTopViewMode = TopViewMode.thirdPerson;
            Cursor.lockState = CursorLockMode.Locked;
            currentObject = thirdPersonObj.transform;
            GoToPlayer();
            SwitchCamera(thirdPersonCamera);
            pointToClickObj.SetActive(false);
            thirdPersonObj.SetActive(true);
        }

        private void SwitchToTopView()
        {
            currentTopViewMode = TopViewMode.clickToMove;
            Cursor.lockState = CursorLockMode.None;
            currentObject = pointToClickObj.transform;

            SwitchCamera(topviewCamera);
            thirdPersonObj.SetActive(false);
            pointToClickObj.SetActive(true);
        }

        private void MoveCamera()
        {
            if (!topviewCamera.gameObject.activeInHierarchy || currentTopViewMode == TopViewMode.thirdPerson) return;

            float mouseVelocity = Time.unscaledDeltaTime * speed;
            // Only move the camera when the cursor is insize the window

            DragCamera(mouseVelocity);
            if (panWithinScreenOnly ?
                mousePos.x >= 0 && mousePos.x <= Screen.width &&
                mousePos.y >= 0 && mousePos.y <= Screen.height : true)
            {
                PanCamera(mouseVelocity);
            }
        }

        private void DragCamera(float mouseVelocity)
        {
            Vector3 msPos = GetInputAxis();
            if (Input.GetMouseButtonDown(1))
            {
                isDragging = true;
                oldPanningPos = topviewCamera.transform.eulerAngles;
                panOrigin = maincam.ScreenToViewportPoint(msPos);
                //        Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            if (Input.GetMouseButton(1))
            {
                Vector3 screenPos = maincam.ScreenToViewportPoint(msPos) - panOrigin;
                screenPos.z = 0;

                Vector3 switchedPos = screenPos;
                switchedPos.x = screenPos.y;
                switchedPos.y = screenPos.x;
                //screenPos.y = 0;

                // topviewCamera.transform.eulerAngles = oldPanningPos + -switchedPos * speed;

                topviewCamera.transform.eulerAngles = GetLookEuler();
            }

            if (Input.GetMouseButtonUp(1))
            {
                //     Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                isDragging = false;
            }
        }

        private void DragCamera(float mouseVelocity, KeyCode code, Goat.InputManager.KeyMode keyMode)
        {
            if (code == KeyCode.Mouse0 && keyMode.HasFlag(InputManager.KeyMode.Down))
            {
                isDragging = true;
                oldPanningPos = currentObject.position;
                panOrigin = maincam.ScreenToViewportPoint(mousePos);
            }

            if (code == KeyCode.Mouse0 && keyMode.HasFlag(InputManager.KeyMode.Pressed))
            {
                Vector3 screenPos = maincam.ScreenToViewportPoint(mousePos) - panOrigin;
                screenPos.z = screenPos.y;
                screenPos.y = 0;
                Debug.LogFormat("{0}+{1}+{2}", oldPanningPos, -screenPos, speed);
                currentObject.position = oldPanningPos + -screenPos * speed;
            }

            if (code == KeyCode.Mouse0 && keyMode.HasFlag(InputManager.KeyMode.Up))
            {
                isDragging = false;
            }
        }

        private void PanCamera(float mouseVelocity)
        {
            if (isDragging) return;
            if (GridUIManager.IsUIActive) return;
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