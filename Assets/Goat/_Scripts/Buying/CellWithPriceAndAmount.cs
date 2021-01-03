using TMPro;
using UnityEngine;

public class CellWithPriceAndAmount : CellWithPrice
{
    [SerializeField] private RectTransform amountBackground;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private bool inventoryAmount;

    public override void Setup(Buyable buyable)
    {
        if (inventoryAmount)
        {
            Debug.LogError("You're using the wrong setup");
            return;
        }
        base.Setup(buyable);
        buyable.AmountChanged += OnAmountChanged;
        ChangeAmountText(buyable.Amount);
        ChangeText((int)buyable.Price, priceText, priceBackground);
    }

    public void Setup(Buyable buyable, int amount)
    {
        if (!inventoryAmount)
        {
            Debug.LogError("You're using the wrong setup");
            return;
        }

        base.Setup(buyable);
        ChangeAmountText(amount);
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