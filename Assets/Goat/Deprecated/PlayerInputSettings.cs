using UnityEngine;

namespace Goat.CameraControls
{
    /// <summary>
    /// Holds keycodes for all input of the player
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerInputSettings", menuName = "ScriptableObjects/PlayerInputSettings")]
    public class PlayerInputSettings : ScriptableObject
    {
        [SerializeField] private KeyCode moveForward;
        [SerializeField] private KeyCode moveLeft;
        [SerializeField] private KeyCode moveDownward;
        [SerializeField] private KeyCode moveRight;

        [SerializeField] private KeyCode run;

        [SerializeField] private KeyCode toggleMouse;
        [SerializeField] private KeyCode topViewMode;
        [SerializeField] private KeyCode thirdPersonMode;
        [SerializeField] private KeyCode stationaryMode;
        [SerializeField] private KeyCode switchTopViewMode;

        public KeyCode MoveForward => moveForward;
        public KeyCode MoveDownward => moveDownward;
        public KeyCode MoveLeft => moveLeft;
        public KeyCode MoveRight => moveRight;
        public KeyCode Run => run;
        public KeyCode ToggleMouse => toggleMouse;
        public KeyCode TopViewMode => topViewMode;
        public KeyCode ThirdPersonMode => thirdPersonMode;
        public KeyCode StationaryMode => stationaryMode;
        public KeyCode SwitchTopViewMode => switchTopViewMode;

    }
}