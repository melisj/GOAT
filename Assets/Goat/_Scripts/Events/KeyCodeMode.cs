using UnityEngine;
using static Goat.InputManager;

namespace Goat.Events
{
    public class KeyCodeMode : IPairable<KeyCode, KeyMode>
    {
        public KeyCode Item1 { get => _item1; set => _item1 = value; }
        public KeyMode Item2 { get => _item2; set => _item2 = value; }

        [SerializeField]
        private KeyCode _item1;
        [SerializeField]
        private KeyMode _item2;

        public void Deconstruct(out KeyCode item1, out KeyMode item2)
        {
            item1 = Item1; item2 = Item2;
        }
    }
}