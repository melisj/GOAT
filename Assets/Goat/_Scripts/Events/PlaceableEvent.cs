using UnityAtoms;
using UnityEngine;

namespace Goat.Events
{
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/PlaceableEvent", fileName = "PlaceableEvent")]
    public class PlaceableEvent : AtomEvent<Placeable> { }
}