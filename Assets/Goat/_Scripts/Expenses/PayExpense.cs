﻿using Goat.Events;
using UnityEngine;

namespace Goat.Expenses
{
    public abstract class PayExpense : EventListenerInt
    {
        [SerializeField] protected Money money;
        [SerializeField] protected ExpenseEvent expenseEvent;

        /// <summary>
        /// Pays the expense
        /// If not able to pay, consequences should happen
        /// Create an Expense and send it to expenseEvent
        /// </summary>
        public abstract void Pay();

        /// <summary>
        /// Should be linked with the UI to be able to pay
        /// </summary>
        public abstract void OnFullPay();
    }
}