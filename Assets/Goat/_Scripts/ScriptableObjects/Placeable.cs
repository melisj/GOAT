using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityAtoms.BaseAtoms;

public class Placeable : Buyable
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private IntVariable totalBeautyPoints;
    [SerializeField] private int beautyPoints;
    [SerializeField] private bool createsWallsAround = true;
    public GameObject Prefab => prefab;
    public bool CreatesWallsAround => createsWallsAround;
    public int BeautyPoints => beautyPoints;
}