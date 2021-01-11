using Goat.Expenses;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Goat.Pooling;

public class ExpenseCell : UICell, IPoolObject
{
    [SerializeField] private TextMeshProUGUI priceTM;
    [SerializeField] private TextMeshProUGUI dateTM;
    [SerializeField] private Money money;
    private Action onPay;
    public int Price { get; private set; }
    public int PoolKey { get; set; }
    public ObjectInstance ObjInstance { get; set; }

    public void Setup(Expense expense, SelectedCells selectedCells)
    {
        this.selectedCellsHolder = selectedCells;
        Price = expense.Price;
        priceTM.text = expense.Price.ToString();
        dateTM.text = expense.Date;
        onPay = expense.OnFullPay;
        OnClick(null);
    }

    public void Pay()
    {
        money.Amount -= Price;
        onPay.Invoke();
        PoolManager.Instance.ReturnToPool(gameObject);
    }

    public void OnGetObject(ObjectInstance objectInstance, int poolKey)
    {
        ObjInstance = objectInstance;
        PoolKey = poolKey;
    }

    public void OnReturnObject()
    {
        gameObject.SetActive(false);
    }
}