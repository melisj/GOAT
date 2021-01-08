using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "ScriptableObjects/Buyable/Building")]
public class Building : Placeable
{
    [SerializeField] private bool placeableWithoutFloor;

    public bool PlaceableWithoutFloor => placeableWithoutFloor;
}