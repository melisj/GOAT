using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterMeshes", menuName = "ScriptableObjects/RandomNPCData/CharacterMeshes")]
public class CharacterMeshes : ScriptableObject
{
    public List<Mesh> characterMeshes = new List<Mesh>();
}
