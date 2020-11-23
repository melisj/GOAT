using Sirenix.OdinInspector;
using UnityEngine;

public class Buyable : SerializedScriptableObject
{
    [SerializeField, FoldoutGroup("Base Buyable data")] private float price;
    [SerializeField, FoldoutGroup("Base Buyable data"), PreviewField(Alignment = ObjectFieldAlignment.Left)] private Sprite image;
    [SerializeField, FoldoutGroup("Base Buyable data"), Multiline] private string summary;

    public float Price => price;

    public Sprite Image => image;
    public string Summary => summary;
}