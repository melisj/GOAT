using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class Buyable : SerializedScriptableObject
{
    [SerializeField, FoldoutGroup("Base Buyable data")] private Money money;
    [SerializeField, FoldoutGroup("Base Buyable data")] private float price;
    [SerializeField, FoldoutGroup("Base Buyable data"), PreviewField(Alignment = ObjectFieldAlignment.Left)] private Sprite image;
    [SerializeField, FoldoutGroup("Base Buyable data")] private Mesh mesh;
    [SerializeField, FoldoutGroup("Base Buyable data"), Multiline] private string summary;
    [SerializeField, FoldoutGroup("Base Buyable data")] private int amount;
    [SerializeField, FoldoutGroup("Base Buyable data")] private int deliveryTime;

    public int DeliveryTime => deliveryTime;

    public event EventHandler<int> AmountChanged;

    public Money Money => money;
    private int oldAmount = 0;
    public int OldAmount => oldAmount;

    public float Price => price;

    /// <summary>
    /// Buys the buyable
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="price">If not set, you use the default price</param>
    public void Buy(int amount, float price = -1, bool payNow = true, bool deliverNow = true)
    {
        price = price < 0 ? Price : price;
        float total = this.money.Amount - (price * amount);
        float newMoney = total < 0 ? this.money.Amount / price : total;

        if (payNow)
            this.money.Amount = newMoney;
        if (deliverNow)
            Amount += amount;
    }

    /// <summary>
    /// Sells the buyable
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="price">If not set, you use the default price</param>
    public void Sell(int amount, float price = -1, bool payNow = true, bool deliverNow = true)
    {
        price = price < 0 ? Price : price;
        int total = Amount - amount;
        int newTotal = total <= 0 ? Amount : total;

        if (deliverNow)
            Amount -= newTotal;
        if (payNow)
            this.money.Amount += newTotal * price;
    }

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
            if (Application.isPlaying)
                AmountChanged?.Invoke(this, value);
        }
    }

    public Sprite Image => image;
    public Mesh Mesh => mesh;
    public string Summary => summary;
}