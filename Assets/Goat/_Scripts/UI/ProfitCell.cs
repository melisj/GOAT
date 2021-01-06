using UnityEngine;
using TMPro;
using System;

public class ProfitCell : MonoBehaviour
{
    [SerializeField] private TotalBudgetCell totalIncome;
    [SerializeField] private TotalBudgetCell totalExpenses;
    [SerializeField] private TextMeshProUGUI profitTM;

    private void OnEnable()
    {
        totalExpenses.OnTotalChanged += TotalChanged;
        totalIncome.OnTotalChanged += TotalChanged;
    }

    private void OnDisable()
    {
        totalExpenses.OnTotalChanged -= TotalChanged;
        totalIncome.OnTotalChanged -= TotalChanged;
    }

    private void OnDestroy()
    {
        OnDisable();
    }

    private void TotalChanged(object sender, EventArgs e)
    {
        ChangeText();
    }

    private void ChangeText()
    {
        profitTM.text = (totalIncome.TotalValue - totalExpenses.TotalValue).ToString("N");
    }
}