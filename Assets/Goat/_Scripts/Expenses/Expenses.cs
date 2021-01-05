using Goat.Events;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Goat.Expenses
{
    public class Expenses : EventListenerExpenseEvent
    {
        [SerializeField, ReadOnly] private List<Expense> expenses = new List<Expense>();
        public List<Expense> AllExpenses => expenses;

        public override void OnEventRaised(Expense value)
        {
            expenses.Add(value);
        }
    }
}