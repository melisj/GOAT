using UnityAtoms;
using UnityEngine;

namespace Goat.Events
{
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/KeyCodeModeEvent", fileName = "KeyCodeModeEvent")]
    public class KeyCodeModeEvent : AtomEvent<KeyCodeMode> { }
}