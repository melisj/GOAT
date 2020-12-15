using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterJobMeshes", menuName = "ScriptableObjects/RandomNPCData/CharacterJobMeshes")]
public class CharacterJobMeshes : ScriptableObject
{
    public List<Mesh> characterJobMeshes = new List<Mesh>();
}
