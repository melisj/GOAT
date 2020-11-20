using Goat.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Goat.Selling
{
    public class SellingUI : MonoBehaviour
    {
        [Header("Resource UI")]
        [SerializeField] private Resource resource;
        [SerializeField] private TextMeshProUGUI resourceName;
        [SerializeField] private TextMeshProUGUI stock;
        [SerializeField] private TextMeshProUGUI basePrice;
        [SerializeField] private Image resourceImage;
        [Header("Data")]
        [SerializeField] private TextMeshProUGUI demand;
        [SerializeField] private TextMeshProUGUI recentPrice;
        [Header("Validate UI")]
        [SerializeField] private TextMeshProUGUI balance;
        [SerializeField] private TextMeshProUGUI capacity;
        [Header("Calculated UI")]
        [SerializeField] private TextMeshProUGUI totalProfit;
        [Header("Inputs")]
        [SerializeField] private TMP_InputField priceInput;
        [SerializeField] private TMP_InputField amountInput;
        [Header("Buttons")]
        [SerializeField] private Button minPriceButton;
        [SerializeField] private Button maxPriceButton;
        [SerializeField] private Button minAmountButton;
        [SerializeField] private Button maxAmountButton;
        [SerializeField] private Button sellButton;

        private float currentPrice;
        private float minPrice; //Data of economy
        private float maxPrice; //Data of economy
        private int currentAmount;
        private Resource previousResource;

        public Resource Resource
        {
            get => resource;
            set
            {
                resource = value;
                if (previousResource != resource)
                {
                    OnResourceChanged();
                }
            }
        }

        private void Awake()
        {
            previousResource = resource;
            SetupUI();
        }

        private void OnResourceChanged()
        {
            //Unsub event of previous resource
            //Setup the text and image again
            //Reset the inputfields
            RemoveListeners();
            previousResource = resource;
            SetupUI();
        }

        private void SetupUI()
        {
            SetupResourceUI();
            SetupDataUI();
            SetupInputUI();
            SetButtonUI();
            SetupCalculatedUI();
            SetupValidateUI();
        }

        private void SetupResourceUI()
        {
            resourceName.text = resource.ResourceType.ToString();
            stock.text = resource.Amount.ToString();
            basePrice.text = resource.ResValue.ToString();
            resourceImage.sprite = resource.Image;
            resource.AmountChanged += Resource_AmountChanged;
        }

        private void SetupDataUI()
        {
            /////???????????????????? DO SOME GRAPH SHIT
        }

        private void SetupValidateUI()
        {
            //balance.text = GameManager.Instance.Money.ToString();
            //?? ?? = selectedShelf.GetComponent<??>();
            //capacity.text = ??.storedAmount + " / " + ??.maxAmount;
        }

        private void SetupCalculatedUI()
        {
            totalProfit.text = (currentPrice * currentAmount).ToString();
        }

        private void SetupInputUI()
        {
            priceInput.onValueChanged.AddListener(OnEndEditPrice);
            amountInput.onValueChanged.AddListener(OnEndEditAmount);
            MinPrice();
            MinAmount();
        }

        private void SetButtonUI()
        {
            minPriceButton.onClick.AddListener(MinPrice);
            maxPriceButton.onClick.AddListener(MaxPrice);
            minAmountButton.onClick.AddListener(MinAmount);
            maxAmountButton.onClick.AddListener(MaxAmount);
            sellButton.onClick.AddListener(ConfirmSale);
        }

        #region EventMethods

        private void OnEndEditPrice(string s)
        {
            resource.ResValue = int.Parse(s);
            currentPrice = resource.ResValue;
            SetupCalculatedUI();
        }

        private void MaxPrice()
        {
            priceInput.text = (resource.ResValue * 5).ToString();
        }

        private void MinPrice()
        {
            priceInput.text = resource.ResValue.ToString();
        }

        private void MaxAmount()
        {
            amountInput.text = resource.Amount.ToString();
        }

        private void MinAmount()
        {
            amountInput.text = resource.Amount > 0 ? 1.ToString() : resource.Amount.ToString();
        }

        private void ConfirmSale()
        {
            Debug.Log("Sale confirmed");
            resource.Amount -= currentAmount;
        }

        private void Resource_AmountChanged(object sender, int amount)
        {
            stock.text = resource.Amount.ToString();
        }

        private void OnEndEditAmount(string s)
        {
            currentAmount = int.Parse(s);
            if (currentAmount > resource.Amount)
            {
                currentAmount = resource.Amount;
                amountInput.text = currentAmount.ToString();
            }
            SetupCalculatedUI();
        }

        #endregion EventMethods

        private void RemoveListeners()
        {
            if (previousResource == null) return;
            previousResource.AmountChanged -= Resource_AmountChanged;
            priceInput.onValueChanged.RemoveListener(OnEndEditPrice);
            amountInput.onValueChanged.RemoveListener(OnEndEditAmount);
            minPriceButton.onClick.RemoveListener(MaxAmount);
            maxPriceButton.onClick.RemoveListener(MaxAmount);
            minAmountButton.onClick.RemoveListener(MinAmount);
            maxAmountButton.onClick.RemoveListener(MaxAmount);
            sellButton.onClick.RemoveListener(ConfirmSale);
        }
    }
}