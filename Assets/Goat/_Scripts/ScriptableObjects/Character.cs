using Goat.AI;
using UnityEngine;

[System.Serializable]
public class Character
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private CharacterNames characterNames;
    [SerializeField] private Sprite headShot;

    public Mesh Mesh => mesh;
    public CharacterNames CharacterNames => characterNames;
    public Sprite HeadShot => headShot;
}