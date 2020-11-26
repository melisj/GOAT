using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Used for grid cells that show amounts linked to a buyable
/// </summary>
public class CellWithAmount : MonoBehaviour
{
    [SerializeField] private RectTransform imageIcon;
    [SerializeField] private TextMeshProUGUI amount;

    public void Setup(Buyable buyable)
    {
        ChangeText(buyable.Amount);
        buyable.AmountChanged += Buyable_AmountChanged;
    }

    private void Buyable_AmountChanged(object sender, int e)
    {
        ChangeText(e);
    }

    private void ChangeText(int change)
    {
        float iconWidth = 11 + (26 / 6 * (change.ToString().Length - 1));
        imageIcon.sizeDelta = new Vector2(iconWidth, imageIcon.sizeDelta.y);
        amount.text = change.ToString();
    }
}