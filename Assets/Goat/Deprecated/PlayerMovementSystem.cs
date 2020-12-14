using Goat.CameraControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Player
{
    public class PlayerMovementSystem : MovementSystem
    {
        protected override void Instance_OnInputEvent(KeyCode code, InputManager.KeyMode keyMode, InputMode inputMode)
        {
            if (keyMode.HasFlag(InputManager.KeyMode.Pressed))
            {
                if (inputMode == InputMode.Select | inSelectMode)
                {
                    moveTo = GetMoveDirection();

                    Move();
                }
            }
        }

        protected override Vector3 GetMoveDirection()
        {
            Vector3 moveDir = base.GetMoveDirection();

            //Slow down backwards
            if (moveDir.x < 0)
            {
                moveDir.x *= 0.9f;
            }

            return moveDir;
        }
    }
}