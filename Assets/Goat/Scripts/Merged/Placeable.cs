using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Placeable : Buyable
{
    [SerializeField, AssetList(Path = "Goat/Prefabs/Grid/Resources"), InlineEditor(InlineEditorModes.LargePreview)] private GameObject prefab;

    public GameObject Prefab => prefab;
    
}