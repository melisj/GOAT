using Cinemachine;

namespace Goat.CameraControls
{
    public class FramingTransposerCameraZoom : CameraZoom
    {
        private CinemachineFramingTransposer transposer;
        private float latestZoom = -999;

        private void OnEnable()
        {
            if (transposer && latestZoom > -999)
            {
                transposer.m_CameraDistance = latestZoom;
            }
        }

        private void OnDisable()
        {
            if (transposer && latestZoom > -999)
            {
                transposer.m_CameraDistance = latestZoom;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (baseComp is CinemachineFramingTransposer)
            {
                transposer = (CinemachineFramingTransposer)baseComp;
            }
            latestZoom = minMaxZoom.y / 2;
            transposer.m_CameraDistance = latestZoom;
        }

        private void Update()
        {
            latestZoom += Zoom(transposer.m_CameraDistance);
            transposer.m_CameraDistance = latestZoom;
        }
    }
}