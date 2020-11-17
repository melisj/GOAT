using UnityEngine;

namespace Goat.Camera
{
    /// <summary>
    /// Holds keycodes for all input of the player
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerInputSettings", menuName = "ScriptableObjects/PlayerInputSettings")]
    public class PlayerInputSettings : ScriptableObject
    {
        [SerializeField] private KeyCode moveForward;
        [SerializeField] private KeyCode moveDownward;
        [SerializeField] private KeyCode moveLeft;
        [SerializeField] private KeyCode moveRight;

        [SerializeField] private KeyCode run;
        
        [SerializeField] private KeyCode toggleMouse;
        public KeyCode MoveForward { get => moveForward; set => moveForward = value; }
        public KeyCode MoveDownward { get => moveDownward; set => moveDownward = value; }
        public KeyCode MoveLeft { get => moveLeft; set => moveLeft = value; }
        public KeyCode MoveRight { get => moveRight; set => moveRight = value; }
        public KeyCode Run { get => run; set => run = value; }

        public KeyCode ToggleMouse { get => toggleMouse; set => toggleMouse = value; }
    }
}