using TMPro;
using UnityEngine;

/// <summary>
/// Used for grid cells that show amounts linked to a buyable
/// </summary>
public class CellWithAmount : UICell
{
    [SerializeField] protected RectTransform amountBackground;
    [SerializeField] protected TextMeshProUGUI amountText;
    [SerializeField] private float margin;
    private Buyable buyable;

    public override void Setup(Buyable buyable)
    {
        base.Setup(buyable);
        ChangeText(buyable.Amount);
        this.buyable = buyable;
        buyable.AmountChanged += Buyable_AmountChanged;
    }

    protected void Buyable_AmountChanged(object sender, int e)
    {
        ChangeText(e);
    }

    protected void ChangeText(int change)
    {
        float iconWidth = (amountText.fontSize) + ((amountText.fontSize + margin) * (change.ToString().Length));
        amountBackground.sizeDelta = new Vector2(iconWidth, amountBackground.sizeDelta.y);
        amountText.text = change.ToString();
    }

    private void OnDestroy()
    {
        if (buyable != null)
        {
            buyable.AmountChanged -= Buyable_AmountChanged;
        }
    }
}