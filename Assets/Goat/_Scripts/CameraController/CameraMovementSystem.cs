using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.CameraControls
{
    public class CameraMovementSystem : MovementSystem
    {
        protected override void Instance_OnInputEvent(KeyCode code, InputManager.KeyMode keyMode, InputMode inputMode)
        {
            if (keyMode.HasFlag(InputManager.KeyMode.Pressed))
            {
                moveTo = GetMoveDirection();
                Move();
            }
        }
    }
}