using UnityEngine;
using System.Collections;
using Goat.Events;
using System.Collections.Generic;
using Goat.Farming.Electricity;

namespace Goat.Expenses
{
    public class PayElectricity : PayExpense<List<int>>
    {
        [SerializeField] private ExpenseCosts expenses;
        [SerializeField] private Electricity electricity;
        private readonly List<int> disabledIndexes = new List<int>();

        public override void OnEventRaised(int value)
        {
            Pay();
        }

        public override void OnFullPay(List<int> disabledIndexes)
        {
            for (int i = 0; i < disabledIndexes.Count; i++)
            {
                int index = disabledIndexes[i];
                if (index < electricity.GeneratorInteractables.Count)
                    electricity.EnableGenerator(electricity.GeneratorInteractables[index]);
            }
        }

        public override void Pay()
        {
            int remainingPrice = 0;
            int fullPrice = 0;
            disabledIndexes.Clear();
            for (int i = 0; i < electricity.GeneratorInteractables.Count; i++)
            {
                int price = electricity.GeneratorInteractables[i].PowerProduction * expenses.PowerCost;
                fullPrice += price;
                if (money.CanPay(price))
                {
                    money.Amount -= price;
                }
                else
                {
                    electricity.DisableGenerator(electricity.GeneratorInteractables[i]);
                    disabledIndexes.Add(i);
                    //Can't pay so make an expense and send it to an expenses holder
                    remainingPrice += price;
                }
            }
            onExpenseCreated.Raise(fullPrice);
            if (remainingPrice > 0)
                expenseEvent.Raise(new Expense(remainingPrice, "Electricity", time.GetDate(), () => OnFullPay(disabledIndexes)));
        }
    }
}