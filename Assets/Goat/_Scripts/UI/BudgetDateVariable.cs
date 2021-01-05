using UnityEngine;
using UnityAtoms;

[EditorIcon("atom-icon-lush")]
[CreateAssetMenu(menuName = "Unity Atoms/Variables/BudgetDate", fileName = "BudgetDateVariable")]
public class BudgetDateVariable : ScriptableObject
{
    [SerializeField] private BudgetDateEvent onBudgetDateChanged;
    [SerializeField] private BudgetDate budgetDate;

    public BudgetDate BudgetDate
    {
        get => budgetDate;
        set
        {
            onBudgetDateChanged?.Raise(value);
            budgetDate = value;
        }
    }

    public BudgetDateEvent OnBudgetDateChanged => onBudgetDateChanged;
}