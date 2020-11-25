using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class Buyable : SerializedScriptableObject
{
    [SerializeField, FoldoutGroup("Base Buyable data")] private float price;
    [SerializeField, FoldoutGroup("Base Buyable data"), PreviewField(Alignment = ObjectFieldAlignment.Left)] private Sprite image;
    [SerializeField, FoldoutGroup("Base Buyable data"), Multiline] private string summary;
    [SerializeField, FoldoutGroup("Base Buyable data")] private int amount;
    [SerializeField, FoldoutGroup("Base Buyable data")] private int deliveryTime;

    public int DeliveryTime => deliveryTime;

    public event EventHandler<int> AmountChanged;

    private int oldAmount = 0;
    public int OldAmount => oldAmount;

    public float Price => price;

    public int Amount
    {
        get => amount;
        set
        {
            oldAmount = amount;
            if (amount <= 0 && value <= 0)
            {
                amount = 0;
            }
            else
            {
                amount = value;
            }
            AmountChanged?.Invoke(this, value);
        }
    }

    public Sprite Image => image;
    public string Summary => summary;
}