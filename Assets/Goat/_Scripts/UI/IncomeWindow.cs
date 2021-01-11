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
        [SerializeField] private RectTransform grid;
        [SerializeField] private SelectedCells selectedExpensesHolder;

        private void OnEnable()
        {
            onExpenseGiven.RegisterSafe(this);
        }

        private void OnDisable()
        {
            onExpenseGiven.UnregisterSafe(this);
        }

        public void OnEventRaised(Expense expense)
        {
            CreateCell(expense);
        }

        private void CreateCell(Expense expense)
        {
            GameObject cell = PoolManager.Instance.GetFromPool(expensePrefab, grid);
            ExpenseCell cellScript = cell.GetComponent<ExpenseCell>();

            cellScript.Setup(expense, selectedExpensesHolder);
        }
    }
}