using TMPro;
using UnityEngine;

/// <summary>
/// Used for grid cells that show amounts linked to a buyable
/// </summary>
public class CellWithAmount : UICell
{
    [SerializeField] protected RectTransform imageIcon;
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
        if (change > 999)
        {
            amountText.text = "∞";
            return;
        }
        float iconWidth = (amountText.fontSize) + ((amountText.fontSize + margin) * (change.ToString().Length));
        imageIcon.sizeDelta = new Vector2(iconWidth, imageIcon.sizeDelta.y);
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