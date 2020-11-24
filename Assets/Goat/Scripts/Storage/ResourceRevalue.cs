using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Goat.Storage
{
    public class ResourceRevalue : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputAmount;
        [SerializeField] private TMP_InputField inputPrice;
        [SerializeField] private Button sellButton;
        [SerializeField] private Resource selectedResource;
        private int amountChange;

        private void Awake()
        {
            inputAmount.onValueChanged.AddListener(OnEndEditAmount);
            inputPrice.onValueChanged.AddListener(OnEndEditPrice);
            sellButton.onClick.AddListener(ConfirmSale);
        }

        public void SelectItem(Resource resource)
        {
            Debug.Log("selecting resource " + resource.ResourceType.ToString());
            selectedResource = resource;
        }

        private void OnEndEditAmount(string s)
        {
            amountChange = int.Parse(s);
            if (amountChange > selectedResource.Amount)
            {
                amountChange = selectedResource.Amount;
                inputAmount.text = amountChange.ToString();
            }
        }

        private void OnEndEditPrice(string s)
        {
            selectedResource.ResValue = int.Parse(s);
        }

        private void ConfirmSale()
        {
            selectedResource.Amount -= amountChange;
        }
    }
}