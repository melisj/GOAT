using UnityEngine;

namespace Goat.CameraControls
{
    public class MouseToggle
    {
        [SerializeField] private KeyCode mouseToggleKey;

        private void ToggleMouse(KeyCode code, Goat.InputManager.KeyMode keyMode)
        {
            if (keyMode == InputManager.KeyMode.Down)
            {
                if (code == mouseToggleKey && Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
                else if (code == mouseToggleKey && Cursor.lockState == CursorLockMode.None)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
    }
}