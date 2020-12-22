using Goat.Storage;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryIcon : MonoBehaviour
{
    [SerializeField] private bool hasPrice;
    [SerializeField, ShowIf("hasPrice")] private TextMeshProUGUI priceText;

    [SerializeField] private bool hasAmount;
    [SerializeField, ShowIf("hasAmount")] private TextMeshProUGUI amountText;
    [SerializeField, ShowIf("hasAmount")] private GameObject amountPanel;

    [SerializeField] private TextMeshProUGUI resourceNameText;
    [SerializeField] private Image resourceIcon;
    
    public Button IconButton;

    public void SetIconData(Resource resource, float price, int amount, Action callback = null)
    {
        this.resourceIcon.sprite = resource.Image;
        resourceNameText.text = resource.name; 

        if (priceText)
            priceText.text = price != 0 ? string.Format("Price: {0}", price) : "";

        if (amountText)
        {
            amountText.text = amount.ToString();
            amountPanel.SetActive(amount != 0);
        }

        if(IconButton != null && callback != null) 
            IconButton.onClick.AddListener(() => callback.Invoke());
    }
}
