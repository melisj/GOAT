using System.Collections.Generic;
using UnityAtoms;
using UnityEngine;

[EditorIcon("atom-icon-cherry")]
[CreateAssetMenu(menuName = "Unity Atoms/Events/Vector3HashSetEvent", fileName = "Vector3HashSetEvent")]
public class Vector3HashSetEvent : AtomEvent<WithOwner<HashSet<Vector3>>>
{
}