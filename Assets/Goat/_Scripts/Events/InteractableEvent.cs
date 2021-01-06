using UnityEngine;
using UnityAtoms;
using Goat.Grid.Interactions;

namespace Goat.Events
{
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/InteractableEvent", fileName = "InteractableEvent")]
    public class InteractableEvent : AtomEvent<BaseInteractable> { }
}