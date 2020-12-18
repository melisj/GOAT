using TMPro;
using UnityEngine;

/// <summary>
/// Used for grid cells that show amounts linked to a buyable
/// </summary>
public class CellWithPrice : MonoBehaviour
{
    private const string MoneySign = "$";

    [SerializeField] protected RectTransform imageIcon;
    [SerializeField] protected TextMeshProUGUI amountText;

    public virtual void Setup(Buyable buyable)
    {
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