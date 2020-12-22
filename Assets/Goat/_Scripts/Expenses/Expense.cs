using System;

namespace Goat.Expenses
{
    public class Expense
    {
        private int price;
        private string name;
        private Action onFullPay;

        public Expense(int price, string name, Action onFullPay)
        {
            this.price = price;
            this.name = name;
            this.onFullPay = onFullPay;
        }
    }
}