using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using TMPro;

public class BudgetHolder : MonoBehaviour
{
    [SerializeField] private VoidEvent OnDayChange;
    [SerializeField] private VoidEvent OnMonthChange;
    [SerializeField] private VoidEvent OnYearChange;
    [SerializeField] protected TextMeshProUGUI budgetFactorTM;
    [SerializeField] protected int dayValue, monthValue, yearValue;

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    protected virtual void ChangeText()
    {
    }
}