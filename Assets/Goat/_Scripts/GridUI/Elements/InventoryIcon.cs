using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryIcon : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image resourceIcon;

    public void SetIconData(Sprite resourceIcon, float price, int amount)
    {
        this.resourceIcon.sprite = resourceIcon;
        priceText.text = string.Format("Price: {0}", price);
        amountText.text = string.Format("Amount: {0}", amount);
    }
}
