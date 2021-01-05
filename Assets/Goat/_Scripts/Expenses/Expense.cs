using System;
using UnityEngine;

namespace Goat.Expenses
{
    [System.Serializable]
    public class Expense
    {
        [SerializeField] private int price;
        [SerializeField] private string name;
        [SerializeField] private string date;
        [SerializeField] private Action onFullPay;

        public Expense(int price, string name, string date, Action onFullPay)
        {
            this.price = price;
            this.name = name;
            this.date = date;
            this.onFullPay = onFullPay;
        }

        public int Price => price;
        public string Name => name;
        public string Date => date;
        public Action OnFullPay => onFullPay;
    }
}