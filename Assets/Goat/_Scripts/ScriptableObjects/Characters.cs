using Goat.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterMeshes", menuName = "ScriptableObjects/RandomNPCData/CharacterMeshes")]
public class Characters : ScriptableObject
{
    [SerializeField] private Character[] characters;
    public Character GetCharacter => characters[Random.Range(0, characters.Length)];
}