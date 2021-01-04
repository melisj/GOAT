using Goat.Grid.UI;
using UnityAtoms.BaseAtoms;
using UnityAtoms;
using Goat.Expenses;
using UnityEngine;
using Goat.Events;
using Goat.Pooling;

namespace Goat.UI
{
    public class IncomeWindow : BasicGridUIElement, IAtomListener<Expense>
    {
        [SerializeField] private ExpenseEvent onExpenseGiven;
        [SerializeField] private GameObject expensePrefab;
        [SerializeField] private SelectedCells selectedExpensesHolder;

        public void OnEventRaised(Expense expense)
        {
            CreateCell(expense);
        }

        private void CreateCell(Expense expense)
        {
            GameObject cell = PoolManager.Instance.GetFromPool(expensePrefab);
            ExpenseCell cellScript = cell.GetComponent<ExpenseCell>();

            cellScript.Setup(expense, selectedExpensesHolder);
        }
    }
}