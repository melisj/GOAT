
namespace Goat.Camera
{
    using UnityEngine;

    public class PlayerMovementSystem : MonoBehaviour
    {
        #region Private Fields

        [SerializeField] private Rigidbody playerController;
        [SerializeField] private Transform cameraTrans;

        [SerializeField] private float walkSpeed;
        //[SerializeField] private float crouchSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private float dashSpeed;
        [SerializeField] private float dashUpPower;
        [SerializeField] private float speedSmoothTime;
        [SerializeField] private float stopSmoothTime;
        [SerializeField] private float turnSmoothTime;
        [SerializeField] private bool turnWithMovementOnly;
        [SerializeField]private PlayerInputSystem playerInputSystem;

        //[SerializeField] private int gravity;
        //[SerializeField] private int airControlPercent;
        //[SerializeField] private float jumpHeight;
        //[SerializeField] private LayerMask groundLayer;

        [SerializeField] private bool debugMode;
        [SerializeField] private Vector3 moveTo;
        [SerializeField] private Vector3 moveToDirection;
        [SerializeField] private Vector3 velocity;
        [SerializeField] private float currentSpeed;
        [SerializeField] private float turnSmoothVelocity;
        [SerializeField] private float speedSmoothVelocity;
        [SerializeField] private float velocityY;

        #endregion Private Fields

        #region Public Methods

        private void Update()
        {
            UpdateMovement();
        }

        public void UpdateMovement()
        {
            Move();
        }

        #endregion Public Methods

        #region Private Methods

        private void Move()
        {
            moveTo = playerInputSystem.GetMoveDirection();
            moveToDirection = moveTo.normalized;

            if (turnWithMovementOnly ? moveTo != Vector3.zero : !turnWithMovementOnly)
            {
                float targetRotation = Mathf.Atan2(moveToDirection.x, moveToDirection.z) * Mathf.Rad2Deg +
                                       cameraTrans.eulerAngles.y;
                playerController.transform.eulerAngles = Vector3.up *
                    Mathf.SmoothDampAngle(playerController.transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
            }

            float targetSpeed = GetMoveSpeed() * moveTo.magnitude;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);


            playerController.MovePosition(playerController.position + (playerController.transform.forward * GetMoveSpeed() * moveTo.magnitude) * Time.deltaTime);
            currentSpeed = new Vector2(playerController.velocity.x, playerController.velocity.z).magnitude;
        }

        private float GetMoveSpeed()
        {
            return walkSpeed;
        }

        private void Dash()
        {
            Vector3 forward = playerController.transform.forward;
            forward.y += dashUpPower;
            Vector3 dashVelocity = forward * GetMoveSpeed() * dashSpeed;
            playerController.AddForce(dashVelocity, ForceMode.VelocityChange);
        }

        #endregion Private Methods
    }
}