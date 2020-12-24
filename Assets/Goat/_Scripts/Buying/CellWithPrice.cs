using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used for grid cells that show amounts linked to a buyable
/// </summary>
public class CellWithPrice : UICell
{
    private const string MoneySign = "$";

    [SerializeField] protected RectTransform imageIcon;
    [SerializeField] protected TextMeshProUGUI amountText;
    [SerializeField] private Image borderImage;

    public Image BorderImage => borderImage;

    public override void Setup(Buyable buyable)
    {
        base.Setup(buyable);
        ChangeText((int)buyable.Price);
    }

    protected void Buyable_AmountChanged(object sender, int e)
    {
        ChangeText(e);
    }

    protected void ChangeText(int change)
    {
        float iconWidth = 14 + (26 / 6 * (change.ToString().Length - 1));
        imageIcon.sizeDelta = new Vector2(iconWidth, imageIcon.sizeDelta.y);
        amountText.text = MoneySign + change.ToString();
    }
}