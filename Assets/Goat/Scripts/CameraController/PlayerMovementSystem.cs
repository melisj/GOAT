namespace Goat.CameraControls
{
    using UnityEngine;

    public class PlayerMovementSystem : MonoBehaviour
    {
        #region Private Fields

        [SerializeField] private CharacterController playerController;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private float walkSpeed;
        [SerializeField] private float speedSmoothTime;
        [SerializeField] private float turnSmoothTime;
        [SerializeField] private bool turnWithMovementOnly;
        [SerializeField] private PlayerInputSystem playerInputSystem;

        private Vector3 moveTo;
        private Vector3 moveToDirection;
        private float currentSpeed;
        private float turnSmoothVelocity;
        private float speedSmoothVelocity;

        #endregion Private Fields

        #region Public Methods

        private void Awake()
        {
            InputManager.Instance.OnInputEvent += Instance_OnInputEvent;
        }

        private void Instance_OnInputEvent(KeyCode code, InputManager.KeyMode keyMode, InputMode inputMode)
        {
            if (keyMode == InputManager.KeyMode.Pressed)
            {
                moveTo = GetMoveDirection(code);
            }
            Move();
        }

        public Vector3 GetMoveDirection(KeyCode code, Goat.InputManager.KeyMode keyMode = InputManager.KeyMode.Pressed)
        {
            Vector3 moveDir = Vector3.zero;

            if (code == (KeyCode.S))
            {
                moveDir += (Vector3.back);
            }

            if (code == (KeyCode.W))
            {
                moveDir += (Vector3.forward);
            }

            if (code == (KeyCode.A))
            {
                moveDir += (Vector3.left);
            }

            if (code == (KeyCode.D))
            {
                moveDir += (Vector3.right);
            }

            return moveDir;
        }

        //private void FixedUpdate()
        //{
        //    moveTo = playerInputSystem.GetMoveDirection();
        //    moveToDirection = moveTo.normalized;
        //}

        //private void Update()
        //{
        //    moveTo = playerInputSystem.GetMoveDirection();
        //    moveToDirection = moveTo.normalized;
        //    Move();
        //}

        #endregion Public Methods

        #region Private Methods

        private void Move()
        {
            //if (turnWithMovementOnly ? moveTo != Vector3.zero : !turnWithMovementOnly)
            //{
            //    float targetRotation = Mathf.Atan2(moveToDirection.x, moveToDirection.z) * Mathf.Rad2Deg +
            //        (cameraController.ThirdPersonActive ? cameraController.GetLookEuler().y : 0);
            //    playerController.transform.eulerAngles = Vector3.up *
            //        Mathf.SmoothDampAngle(playerController.transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
            //}

            float targetSpeed = GetMoveSpeed() * moveTo.magnitude;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

            // playerController.Move(transform.forward * currentSpeed * Time.deltaTime);
            transform.Translate(moveTo * currentSpeed * Time.deltaTime);
            //  currentSpeed = new Vector2(playerController.velocity.x, playerController.velocity.z).magnitude;
        }

        private float GetMoveSpeed()
        {
            return walkSpeed;
        }

        #endregion Private Methods
    }
}