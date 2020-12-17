using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnloadLocations", menuName = "ScriptableObjects/RuntimeVariables/UnloadLocations")]
public class UnloadLocations : ScriptableObject
{
    [SerializeField, ReadOnly] private List<Vector3> locations;
    public List<Vector3> Locations => locations;

    [Button]
    private void Clear()
    {
        locations.Clear();
    }
}