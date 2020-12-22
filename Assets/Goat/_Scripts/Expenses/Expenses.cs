using Goat.Events;
using System.Collections.Generic;

namespace Goat.Expenses
{
    public class Expenses : EventListenerExpenseEvent
    {
        private List<Expense> expenses = new List<Expense>();
        public List<Expense> AllExpenses => expenses;

        public override void OnEventRaised(Expense value)
        {
            expenses.Add(value);
        }
    }
}