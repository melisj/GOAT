using Boo.Lang;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Goat
{
    /// <summary>
    /// Holds keycodes for all input of the player
    /// </summary>
    [CreateAssetMenu(fileName = "Inputs", menuName = "InputData")]
    public class InputData : SerializedScriptableObject
    {
        [SerializeField] private Dictionary<KeyCode, InputManager.KeyMode> inputKeys = new Dictionary<KeyCode, InputManager.KeyMode>();

        public Dictionary<KeyCode, InputManager.KeyMode> InputKeys => inputKeys ;
    }
}