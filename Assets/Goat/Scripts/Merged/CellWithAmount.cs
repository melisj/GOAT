using TMPro;
using UnityEngine;

/// <summary>
/// Used for grid cells that show amounts linked to a buyable
/// </summary>
public class CellWithAmount : MonoBehaviour
{
    [SerializeField] protected RectTransform imageIcon;
    [SerializeField] protected TextMeshProUGUI amountText;
    private Buyable buyable;

    public virtual void Setup(Buyable buyable)
    {
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
        float iconWidth = 11 + (26 / 6 * (change.ToString().Length - 1));
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