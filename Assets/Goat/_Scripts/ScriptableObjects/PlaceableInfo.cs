using UnityEngine;

public class PlaceableInfo : MonoBehaviour
{
    [SerializeField] private Placeable placeable;

    public void Setup(Placeable placeable)
    {
        this.placeable = placeable;
    }

    public Placeable Placeable => placeable;
}