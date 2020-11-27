using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GridFiller : MonoBehaviour
{
    [SerializeField, AssetList(Path = "Goat/ScriptableObjects/Data")] private SerializedScriptableObject data;

    public void FillGrid()
    {
    }
}