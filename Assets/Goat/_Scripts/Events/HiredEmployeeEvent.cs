using Goat.AI.Parking;
using UnityAtoms;
using UnityEngine;

namespace Goat.Events
{
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/HiredEmployeeEvent", fileName = "HiredEmployeeEvent")]
    public class HiredEmployeeEvent : AtomEvent<HiredEmployee> { }
}