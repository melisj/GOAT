using UnityAtoms;
using UnityEngine;

namespace Goat.Events
{
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/DeliveryResourceEvent", fileName = "DeliveryResourceEvent")]
    public class DeliveryResourceEvent : AtomEvent<DeliveryResource>
    {
    }
}