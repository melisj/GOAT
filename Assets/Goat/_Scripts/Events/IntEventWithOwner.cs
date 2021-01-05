using UnityAtoms;
using UnityEngine;

[EditorIcon("atom-icon-cherry")]
[CreateAssetMenu(menuName = "Unity Atoms/Events/IntEventWithOwner", fileName = "IntEventWithOwner")]
public class IntEventWithOwner : AtomEvent<WithOwner<int>> { }