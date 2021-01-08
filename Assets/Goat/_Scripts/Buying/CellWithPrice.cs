using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used for grid cells that show amounts linked to a buyable
/// </summary>
public class CellWithPrice : UICell
{
    private const string MONEY_SIGN = "$";

    [SerializeField] protected RectTransform priceBackground;
    [SerializeField] protected TextMeshProUGUI priceText;
    [SerializeField] private float margin = 2;

    public Image BorderImage => border;

    public override void Setup(Buyable buyable)
    {
        base.Setup(buyable);
        ChangeText((int)buyable.Price, priceText, priceBackground);
    }

    protected void ChangeText(int change, TextMeshProUGUI textUI, RectTransform amountHolder)
    {
        ChangeIconWidth(change, textUI, amountHolder);
        textUI.text = MONEY_SIGN + change.ToString();
    }

    protected void ChangeIconWidth(int change, TextMeshProUGUI textUI, RectTransform amountHolder)
    {
        float iconWidth = (textUI.fontSize) + ((textUI.fontSize + margin) * (change.ToString().Length));
        amountHolder.sizeDelta = new Vector2(iconWidth, amountHolder.sizeDelta.y);
    }
}