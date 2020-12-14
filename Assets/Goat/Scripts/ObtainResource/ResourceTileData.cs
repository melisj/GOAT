using Goat.Storage;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceTileData", menuName = "ScriptableObjects/ResourceTileData")]
public class ResourceTileData : ScriptableObject
{
    [SerializeField] private Resource resource;
    [SerializeField] private Mesh resourceTile;
    [SerializeField] private Mesh defaultTile;
    [SerializeField] private int starterAmount;

    public Resource Resource => resource;
    public Mesh ResourceTile => resourceTile;
    public Mesh DefaultTile => defaultTile;
    public int StarterAmount => starterAmount;
}