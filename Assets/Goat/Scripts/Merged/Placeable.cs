using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Placeable : Buyable
{
    [SerializeField] private GameObject prefab;

    public GameObject Prefab => prefab;
}