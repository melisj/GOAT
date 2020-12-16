using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryIcon : MonoBehaviour
{
    [SerializeField] private bool hasPrice;
    [SerializeField, ShowIf("hasPrice")] private TextMeshProUGUI priceText;

    [SerializeField] private bool hasAmount;
    [SerializeField, ShowIf("hasAmount")] private TextMeshProUGUI amountText;

    [SerializeField] private Image resourceIcon;
    
    public Button IconButton;

    public void SetIconData(Sprite resourceIcon, float price, int amount)
    {
        this.resourceIcon.sprite = resourceIcon;
        if(priceText)
            priceText.text = price != 0 ? string.Format("Price: {0}", price) : "";
        if(amountText)
            amountText.text = amount != 0 ? string.Format("Amount: {0}", amount) : "";
    }
}
