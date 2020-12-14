using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Goat.CameraControls
{
    public class PlayerViewRotate : RotateWithMouse
    {
        [SerializeField] private CinemachineVirtualCamera vCam;

        private GameObject thirdPersonObj;
        protected CinemachineComponentBase baseComp;

        private void Awake()
        {
            baseComp = vCam.GetCinemachineComponent(CinemachineCore.Stage.Body);
            thirdPersonObj = vCam.Follow.gameObject;
        }

        protected override void Rotate()
        {
            if (baseComp is Cinemachine3rdPersonFollow)
            {
                Cinemachine3rdPersonFollow follow = (Cinemachine3rdPersonFollow)baseComp;
                follow.CameraDistance = 0;
                thirdPersonObj.transform.eulerAngles = GetLookEuler();
            }
        }
    }
}