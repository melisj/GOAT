namespace Goat.Camera
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

        private void FixedUpdate()
        {
            moveTo = playerInputSystem.GetMoveDirection();
            moveToDirection = moveTo.normalized;
        }

        private void Update()
        {
            Move();
        }

        #endregion Public Methods

        #region Private Methods

        private void Move()
        {
            if (turnWithMovementOnly ? moveTo != Vector3.zero : !turnWithMovementOnly)
            {
                float targetRotation = Mathf.Atan2(moveToDirection.x, moveToDirection.z) * Mathf.Rad2Deg +
                    (cameraController.ThirdPersonActive ? cameraController.GetLookEuler().y : 0);
                playerController.transform.eulerAngles = Vector3.up *
                    Mathf.SmoothDampAngle(playerController.transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
            }

            float targetSpeed = GetMoveSpeed() * moveTo.magnitude;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

            playerController.Move(transform.forward * currentSpeed * Time.deltaTime);
            currentSpeed = new Vector2(playerController.velocity.x, playerController.velocity.z).magnitude;
        }

        private float GetMoveSpeed()
        {
            return walkSpeed;
        }

        #endregion Private Methods
    }
}