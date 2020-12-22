using UnityEngine;
using System.Collections;
using Goat.Events;
using Goat.Storage;

namespace Goat.Expenses
{
    public class PayElectricity : PayExpense
    {
        [SerializeField] private ExpenseCosts expenses;
        [SerializeField] private Electricity electricity;

        public override void OnEventRaised(int value)
        {
            Pay();
        }

        public override void OnFullPay()
        {
            for (int i = 0; i < electricity.GeneratorInteractables.Count; i++)
            {
                electricity.EnableGenerator(electricity.GeneratorInteractables[i]);
            }
        }

        public override void Pay()
        {
            int remainingPrice = 0;
            for (int i = 0; i < electricity.GeneratorInteractables.Count; i++)
            {
                int price = electricity.GeneratorInteractables[i].PowerProduction * expenses.PowerCost;
                if (money.CanPay(price))
                {
                    money.Amount -= price;
                }
                else
                {
                    electricity.DisableGenerator(electricity.GeneratorInteractables[i]);
                    //Can't pay so make an expense and send it to an expenses holder
                    remainingPrice += price;
                }
            }

            expenseEvent.Raise(new Expense(remainingPrice, "Electricity", OnFullPay));
        }
    }
}