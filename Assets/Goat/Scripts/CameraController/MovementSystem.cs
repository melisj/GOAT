namespace Goat.CameraControls
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class MovementSystem : MonoBehaviour
    {
        #region Private Fields

        [SerializeField] private Transform moveableObject;
        [SerializeField] private float walkSpeed;
        [SerializeField] private bool useRotation;
        [SerializeField, ShowIf("useRotation")] private GameObject cameraController;
        [SerializeField, ProgressBar(0.01f, 0.5f)] private float speedSmoothTime;
        [SerializeField, ProgressBar(0.01f, 0.5f)] private float turnSmoothTime;

        protected Vector3 moveTo;
        private Vector3 moveToDirection;
        private float currentSpeed;
        private float turnSmoothVelocity;
        private float speedSmoothVelocity;
        protected bool inSelectMode;

        public Vector3 MoveTo { get => moveTo; }

        #endregion Private Fields

        private void OnEnable()
        {
            InputManager.Instance.InputModeChanged += Instance_InputModeChanged;
            InputManager.Instance.OnInputEvent += Instance_OnInputEvent;
        }

        protected virtual void Instance_InputModeChanged(object sender, InputMode e)
        {
            inSelectMode = e == InputMode.Select;
        }

        protected virtual void Instance_OnInputEvent(KeyCode code, InputManager.KeyMode keyMode, InputMode inputMode)
        {
        }

        protected virtual void OnDisable()
        {
            InputManager.Instance.InputModeChanged -= Instance_InputModeChanged;

            InputManager.Instance.OnInputEvent -= Instance_OnInputEvent;
        }

        protected virtual Vector3 GetMoveDirection()
        {
            Vector3 moveDir = Vector3.zero;

            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            moveDir = input.normalized;

            return moveDir;
        }

        protected void Move()
        {
            if (useRotation ? moveTo != Vector3.zero : useRotation)
            {
                float targetRotation = cameraController.transform.eulerAngles.y;
                moveableObject.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(moveableObject.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
            }

            float targetSpeed = walkSpeed * moveTo.magnitude;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

            moveableObject.Translate(moveTo * currentSpeed * Time.unscaledDeltaTime);
        }
    }
}