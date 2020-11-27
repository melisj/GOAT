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
    [SerializeField] protected RectTransform imageIcon;
    [SerializeField] protected TextMeshProUGUI amountText;

    public virtual void Setup(Buyable buyable)
    {
        ChangeText(buyable.Amount);
        buyable.AmountChanged += Buyable_AmountChanged;
    }

    protected void Buyable_AmountChanged(object sender, int e)
    {
        ChangeText(e);
    }

    protected void ChangeText(int change)
    {
        float iconWidth = 11 + (26 / 6 * (change.ToString().Length - 1));
        imageIcon.sizeDelta = new Vector2(iconWidth, imageIcon.sizeDelta.y);
        amountText.text = change.ToString();
    }
}