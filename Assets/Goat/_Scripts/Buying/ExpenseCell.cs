using Goat.Expenses;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ExpenseCell : UICell
{
    [SerializeField] private TextMeshProUGUI priceTM;
    [SerializeField] private TextMeshProUGUI dateTM;
    private Action onPay;
    public int Price { get; private set; }

    public void Setup(Expense expense, SelectedCells selectedCells)
    {
        this.selectedCellsHolder = selectedCells;
        Price = expense.Price;
        priceTM.text = expense.Price.ToString();
        dateTM.text = expense.Date;
        onPay = expense.OnFullPay;
    }

    public void Pay()
    {
        onPay.Invoke();
    }
}