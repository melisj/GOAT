using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeBudgetDateOnClick : MonoBehaviour
{
    [SerializeField] private BudgetDateVariable budgetDate;
    [SerializeField] private BudgetDate setDate;
    [SerializeField] private Button button;
    [SerializeField] private Image left, middle, right;
    [SerializeField] private BorderSprites unselectedSprites;
    [SerializeField] private BorderSprites selectedSprites;

    private void OnEnable()
    {
        budgetDate.OnBudgetDateChanged.RegisterSafe(ChangeSprites);
    }

    private void OnDisable()
    {
        budgetDate.OnBudgetDateChanged.UnregisterSafe(ChangeSprites);
    }

    private void Awake()
    {
        button.onClick.AddListener(SetBudgetDate);
    }

    private void SetBudgetDate()
    {
        budgetDate.BudgetDate = setDate;
    }

    private void ChangeSprites(BudgetDate date)
    {
        bool selected = date == setDate;

        left.sprite = selected ? selectedSprites.Left : unselectedSprites.Left;
        middle.sprite = selected ? selectedSprites.Middle : unselectedSprites.Middle;
        right.sprite = selected ? selectedSprites.Right : unselectedSprites.Right;
    }
}