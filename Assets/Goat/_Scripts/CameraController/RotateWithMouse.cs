using Sirenix.OdinInspector;
using UnityEngine;

namespace Goat.CameraControls
{
    public class RotateWithMouse : MonoBehaviour
    {
        [SerializeField] private float sensitivity;
        [SerializeField] private Vector2 pitchMinMax;
        [SerializeField, ProgressBar(0.1f, 0.5f)] private float rotSmoothTime;
        private float yaw;
        private float pitch;
        private Vector3 currentRotation;
        private Vector3 rotSmoothVel;

        protected virtual void Update()
        {
            Rotate();
        }

        protected Vector2 GetInputAxis()
        {
            Vector2 mouseAxis = Vector2.zero;
            mouseAxis.x = Input.GetAxis("Mouse X");
            mouseAxis.y = Input.GetAxis("Mouse Y");

            return mouseAxis;
        }

        protected Vector3 GetLookEuler()
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

        protected virtual void Rotate()
        {
        }
    }
}