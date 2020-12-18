using UnityEngine;

namespace Goat.CameraControls
{
    using Goat.Events;

    public class MouseToggle : EventListenerKeyCodeModeEvent
    {
        [SerializeField] private KeyCode mouseToggleKey;

        public override void OnEventRaised(KeyCodeMode value)
        {
            KeyCode code = KeyCode.None;
            KeyMode mode = KeyMode.None;

            value.Deconstruct(out code, out mode);

            ToggleMouse(code, mode);
        }

        private void ToggleMouse(KeyCode code, KeyMode keyMode)
        {
            if (keyMode == KeyMode.Down)
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