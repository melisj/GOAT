using TMPro;
using UnityEngine;

public class CellWithPriceAndAmount : CellWithPrice
{
    [SerializeField] private RectTransform amountBackground;
    [SerializeField] private TextMeshProUGUI amountText;

    public override void Setup(Buyable buyable)
    {
        base.Setup(buyable);
        buyable.AmountChanged += OnAmountChanged;
        ChangeAmountText(buyable.Amount);
        ChangeText((int)buyable.Price, priceText, priceBackground);
    }

    private void ChangeAmountText(int change)
    {
        ChangeIconWidth(change, amountText, amountBackground);
        amountText.text = change.ToString();
    }

    private void OnAmountChanged(object sender, int e)
    {
        ChangeAmountText(e);
    }
}