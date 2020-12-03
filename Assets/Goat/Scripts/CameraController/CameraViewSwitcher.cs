using Cinemachine;
using Goat.Grid.UI;
using System;
using UnityEngine;

namespace Goat.CameraControls
{
    public enum TopViewMode
    {
        thirdPerson = 0,
        clickToMove = 1
    }

    public class CameraViewSwitcher : MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField] private Camera maincam;
        [SerializeField] private CinemachineVirtualCamera thirdPersonCamera;
        [SerializeField] private CinemachineVirtualCamera topviewCamera;
        [SerializeField] private TopViewMode currentTopViewMode;
        [SerializeField] private GameObject pointToClickObj;
        [SerializeField] private GameObject thirdPersonObj;

        private Transform currentObject;
        private CinemachineVirtualCamera currentActiveCamera;

        public bool ThirdPersonActive => currentActiveCamera == thirdPersonCamera;

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
            if (code == KeyCode.Home && keyMode == InputManager.KeyMode.Down)
            {
                currentObject.position = Vector3.zero;
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

        #endregion Unity Methods

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
    }
}