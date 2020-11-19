namespace Goat.CameraControls
{
    using System;
    using UnityEngine;

    public class PlayerInputSystem : MonoBehaviour
    {
        #region Private Fields

        [SerializeField] private PlayerInputSettings playerInputSettings;
        [SerializeField] private Vector3 direction;
    
        #endregion Private Fields

        #region Public Enums

        [Flags]
        public enum ManeuverType
        {
            None = 0,
            Run = 1
        }

        #endregion Public Enums

        #region Public Methods

        /// <summary>
        /// Returns the move direction based on the movement input
        /// </summary>
        public Vector3 GetMoveDirection()
        {
            Vector3 moveDir = Vector3.zero;

            if (Input.GetKey(playerInputSettings.MoveDownward))
            {
                moveDir += (Vector3.back);
            }

            if (Input.GetKey(playerInputSettings.MoveForward))
            {
                moveDir += (Vector3.forward);
            }

            if (Input.GetKey(playerInputSettings.MoveLeft))
            {
                moveDir += (Vector3.left);
            }

            if (Input.GetKey(playerInputSettings.MoveRight))
            {
                moveDir += (Vector3.right);
            }

            return moveDir;
        }


        /// <summary>
        /// Returns maneuverType based on input
        /// </summary>
        public ManeuverType ManeuverInput()
        {
            ManeuverType type = ManeuverType.None;

            if (Input.GetKey(playerInputSettings.Run))
            {
                type |= ManeuverType.Run;
            }

            return type;
        }

        #endregion Public Methods
    }
}