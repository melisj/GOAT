using UnityAtoms;
using UnityEngine;

[EditorIcon("atom-icon-cherry")]
[CreateAssetMenu(menuName = "Unity Atoms/Events/Vector3OwnerEvent", fileName = "Vector3OwnerEvent")]
public class Vector3OwnerEvent : AtomEvent<WithOwner<Vector3>>
{
}