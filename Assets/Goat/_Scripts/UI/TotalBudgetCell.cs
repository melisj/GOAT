using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using System;

public class TotalBudgetCell : MonoBehaviour
{
    [SerializeField] private BudgetCell[] budgetCells;
    [SerializeField] private TextMeshProUGUI totalTM;
    [SerializeField, ReadOnly] private int totalValue;
    public int TotalValue => totalValue;

    public event EventHandler OnTotalChanged;

    private void Awake()
    {
        totalTM.text = totalValue.ToString("N0");
    }

    private void OnEnable()
    {
        for (int i = 0; i < budgetCells.Length; i++)
        {
            budgetCells[i].OnValueChanged += TotalBudgetCell_OnValueChanged;
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < budgetCells.Length; i++)
        {
            budgetCells[i].OnValueChanged -= TotalBudgetCell_OnValueChanged;
        }
    }

    private void OnDestroy()
    {
        OnDisable();
    }

    private void TotalBudgetCell_OnValueChanged(object sender, EventArgs e)
    {
        totalValue = 0;
        for (int i = 0; i < budgetCells.Length; i++)
        {
            totalValue += budgetCells[i].GetSelectedBudget();
        }
        ChangeText();
    }

    private void ChangeText()
    {
        totalTM.text = totalValue.ToString("N0");
        OnTotalChanged.Invoke(this, null);
    }
}