using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using TMPro;
using Sirenix.OdinInspector;
using System;

public enum BudgetDate
{
    today,
    month,
    year
}

public class BudgetCell : MonoBehaviour
{
    [Title("Date Events")]
    [SerializeField] private IntEvent onDayChange;
    [SerializeField] private IntEvent onMonthChange;
    [SerializeField] private IntEvent onYearChange;
    [Title("Value data")]
    [SerializeField] private IntEvent budgetValueEvent;
    [SerializeField] private BudgetDateVariable selectedDate;
    [SerializeField] private TextMeshProUGUI budgetCellTM;
    [SerializeField, ReadOnly] private int dayValue, monthValue, yearValue;

    public event EventHandler OnValueChanged;

    private void OnEnable()
    {
        onDayChange.RegisterSafe((int _) => ResetValue(out dayValue));
        onMonthChange.RegisterSafe((int _) => ResetValue(out monthValue));
        onYearChange.RegisterSafe((int _) => ResetValue(out yearValue));
        budgetValueEvent.RegisterSafe(AddValue);
        selectedDate.OnBudgetDateChanged.RegisterSafe((BudgetDate date) => ChangeText());
    }

    private void OnDisable()
    {
        onDayChange.UnregisterAllSafe();
        onMonthChange.UnregisterAllSafe();
        onYearChange.UnregisterAllSafe();
        budgetValueEvent.UnregisterAllSafe();
    }

    private void ResetValue(out int value)
    {
        value = 0;
    }

    private void AddValue(int amount)
    {
        dayValue += amount;
        monthValue += amount;
        yearValue += amount;
        ChangeText();
    }

    public int GetSelectedBudget()
    {
        switch (selectedDate.BudgetDate)
        {
            case BudgetDate.today: return dayValue;
            case BudgetDate.month: return monthValue;
            case BudgetDate.year: return yearValue;
        }
        return 0;
    }

    private void ChangeText()
    {
        budgetCellTM.text = GetSelectedBudget().ToString("N");
        OnValueChanged?.Invoke(this, null);
    }
}