using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.CameraControls
{
    public class CameraMovementSystem : MovementSystem
    {
        protected override void OnInput(KeyCode code, KeyMode keyMode)
        {
            if (keyMode.HasFlag(KeyMode.Pressed))
            {
                moveTo = GetMoveDirection();
                Move();
            }
        }
    }
}