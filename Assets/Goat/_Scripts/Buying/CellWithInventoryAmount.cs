using TMPro;
using UnityEngine;

public class CellWithInventoryAmount : UICell
{
    [SerializeField] protected RectTransform amountBackground;
    [SerializeField] protected TextMeshProUGUI amountText;
    [SerializeField] private float margin;
    public int Amount { get; private set; }

    public void Setup(Buyable buyable, int amount)
    {
        base.Setup(buyable);
        Amount = amount;
        ChangeText(amount);
    }

    public void Setup(int amount)
    {
        Amount = amount;
        ChangeText(amount);
    }

    private void ChangeText(int amount)
    {
        ChangeIconWidth(amount, amountText, amountBackground);
        amountText.text = amount.ToString();
    }

    protected void ChangeIconWidth(int change, TextMeshProUGUI textUI, RectTransform amountHolder)
    {
        float iconWidth = (textUI.fontSize) + ((textUI.fontSize + margin) * (change.ToString().Length));
        amountHolder.sizeDelta = new Vector2(iconWidth, amountHolder.sizeDelta.y);
    }
}