using Goat.AI;
using UnityEngine;

[System.Serializable]
public class Character
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private CharacterNames characterNames;

    public Mesh Mesh => mesh;
    public CharacterNames CharacterNames => characterNames;
}