using Goat.Storage;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityAtoms.BaseAtoms;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class Buyable : SerializedScriptableObject
{
    private const string folderPath = "/Goat/Textures/UI/MeshImages/Resources/";

    [SerializeField, FoldoutGroup("Base Buyable data"), ReadOnly] private int id;
    [SerializeField, FoldoutGroup("Base Buyable data")] private IntEvent onPurchase;
    [SerializeField, FoldoutGroup("Base Buyable data")] private IntEvent onRefund;
    [SerializeField, FoldoutGroup("Base Buyable data"), PreviewField(Alignment = ObjectFieldAlignment.Left), ReadOnly] private Sprite image;
    [SerializeField, FoldoutGroup("Base Buyable data")] private Money money;
    [SerializeField, FoldoutGroup("Base Buyable data")] private float price;
    [SerializeField, FoldoutGroup("Base Buyable data")] private bool interchangeableMesh;
    [SerializeField, FoldoutGroup("Base Buyable data"), ShowIf("interchangeableMesh"), PreviewField(Alignment = ObjectFieldAlignment.Left)] private Mesh[] mesh;
    [SerializeField, FoldoutGroup("Base Buyable data"), Multiline] private string summary;
    [SerializeField, FoldoutGroup("Base Buyable data")] private int amount;
    [SerializeField, FoldoutGroup("Base Buyable data")] private int starterAmount;
    [SerializeField, FoldoutGroup("Base Buyable data")] private bool deliverable;
    [SerializeField, FoldoutGroup("Base Buyable data"), ShowIf("deliverable")] private int deliveryTime;

    public int DeliveryTime => deliveryTime;

    public event EventHandler<int> AmountChanged;

    public bool Deliverable => deliverable;

    public int StarterAmount => starterAmount;

    public int ID { get { return id; } set { id = value; } }

    public Money Money => money;
    private int oldAmount = 0;
    public int OldAmount => oldAmount;

    public float Price => price;

    public void OnValidate()
    {
        image = Resources.Load<Sprite>(name);
    }

    /// <summary>
    /// Buys the buyable
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="price">If not set, you use the default price</param>
    public virtual void Buy(int amount, float price = -1, bool payNow = true, bool deliverNow = true)
    {
        price = price < 0 ? Price : price;
        float total = this.money.Amount - (price * amount);
        float newMoney = total < 0 ? this.money.Amount / price : total;

        if (payNow)
        {
            this.money.Amount = newMoney;
            onPurchase.Raise((int)price);
        }
        if (deliverNow)
            Amount += amount;
    }

    public bool CanBuy(int amount, float price = -1)
    {
        price = price < 0 ? Price : price;
        float total = this.money.Amount - (price * amount);
        return total < 0;
    }

    /// <summary>
    /// Sells the buyable
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="price">If not set, you use the default price</param>
    public void Sell(int amount, float price = -1, bool payNow = true, bool deliverNow = true)
    {
        price = price < 0 ? Price * 0.75f : price;
        int total = Amount - amount;
        int newTotal = total <= 0 ? Amount : amount;

        if (deliverNow)
            Amount = newTotal;
        if (payNow)
        {
            onRefund.Raise((int)(newTotal * price));
            this.money.Amount += newTotal * price;
        }
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
                AmountChanged?.Invoke(this, amount);
        }
    }

    public Sprite Image => image;
    public Mesh[] Mesh => mesh;
    public string Summary => summary;
}