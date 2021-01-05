using System;
using UnityEngine;

namespace Goat.Expenses
{
    [System.Serializable]
    public class Expense
    {
        [SerializeField] private int price;
        [SerializeField] private string name;
        [SerializeField] private Action onFullPay;

        public Expense(int price, string name, Action onFullPay)
        {
            this.price = price;
            this.name = name;
            this.onFullPay = onFullPay;
        }
    }
}