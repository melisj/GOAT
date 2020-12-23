using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.CameraControls
{
    public class CameraMovementSystem : MovementSystem
    {
        private void Update()
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                moveTo = GetMoveDirection();
                Move();
            }
        }

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