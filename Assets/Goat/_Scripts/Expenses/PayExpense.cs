using Goat.Events;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Goat.Expenses
{
    public abstract class PayExpense<T> : EventListenerInt
    {
        [SerializeField] protected Money money;
        [SerializeField] protected ExpenseEvent expenseEvent;
        [SerializeField] protected TimeOfDay time;
        [SerializeField] protected IntEvent onExpenseCreated;

        /// <summary>
        /// Pays the expense
        /// If not able to pay, consequences should happen
        /// Create an Expense and send it to expenseEvent
        /// </summary>
        public abstract void Pay();

        /// <summary>
        /// Should be linked with the UI to be able to pay
        /// </summary>
        public abstract void OnFullPay(T arg);
    }
}