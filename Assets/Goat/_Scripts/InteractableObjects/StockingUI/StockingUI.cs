using Goat.Events;
using Goat.Grid.UI;
using Goat.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Goat.Events;

// Original Author: Stanley

namespace Goat.Grid.Interactions.UI
{
    /// <summary>
    /// Script for managing the UI of stocking storage units in the game
    /// </summary>
    public class StockingUI : EventListenerKeyCodeModeEvent
    {
        [Header("Resource UI")]
        [SerializeField] private Resource resource;
        [SerializeField] private GameObject stockingUI;
        [SerializeField] private TextMeshProUGUI resourceName;
        //[SerializeField] private TextMeshProUGUI stock;
        //[SerializeField] private Image resourceImage;
        [Header("Inputs")]
        [SerializeField] private TMP_InputField amountInput;
        [Header("Buttons")]
        [SerializeField] private Button minAmountButton;
        [SerializeField] private Button maxAmountButton;
        [SerializeField] private Button sellButton;

        [Header("References")]
        [SerializeField] private InputModeVariable currentMode;
        [SerializeField] private InteractablesInfo interactableInfo;

        private int currentAmount;
        private Resource previousResource;

        public StorageInteractable Interactable { get { return (StorageInteractable)interactableInfo.CurrentSelected; } }

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

        public override void OnEventRaised(KeyCodeMode value)
        {
            KeyCode code = KeyCode.None;
            KeyMode mode = KeyMode.None;

            value.Deconstruct(out code, out mode);
            OnInput(code, mode);
        }

        private void OnInput(KeyCode code, KeyMode keyMode)
        {
            if (keyMode == KeyMode.Down &&
                currentMode.InputMode == InputMode.Select &&
                stockingUI.activeInHierarchy)
            {
                if (code == KeyCode.KeypadEnter | code == KeyCode.Return)
                {
                    ConfirmStocking();
                }

                if (code == KeyCode.F)
                {
                    MaxAmount();
                }

                if (code == KeyCode.E)
                {
                    MinAmount();
                }
            }
        }

        private void OnResourceChanged()
        {
            RemoveListeners();
            previousResource = resource;
            SetupUI();
        }

        private void SetupUI()
        {
            SetupResourceUI();
            SetupInputUI();
            SetButtonUI();
        }

        private void SetupResourceUI()
        {
            resourceName.text = resource.name.ToString();
            //stock.text = resource.Amount.ToString();
            //resourceImage.sprite = resource.Image;
            resource.AmountChanged += Resource_AmountChanged;
        }

        private void SetupInputUI()
        {
            amountInput.onValueChanged.AddListener(OnEndEditAmount);
            MinAmount();
        }

        private void SetButtonUI()
        {
            minAmountButton.onClick.AddListener(MinAmount);
            maxAmountButton.onClick.AddListener(MaxAmount);
            sellButton.onClick.AddListener(ConfirmStocking);
        }

        #region EventMethods

        private void MaxAmount()
        {
            amountInput.text = Interactable.SpaceLeft.ToString();
        }

        private void MinAmount()
        {
            if (Interactable)
                amountInput.text = Interactable.SpaceLeft > 0 ? 1.ToString() : Interactable.SpaceLeft.ToString();
        }

        private void ConfirmStocking()
        {
            currentAmount = (resource.Amount - currentAmount) <= 0 ? resource.Amount : currentAmount;
            if (Interactable.AddResource(resource, currentAmount, out int actualStoredAmount))
            {
                resource.Amount -= actualStoredAmount;
                stockingUI.SetActive(false);
            }
        }

        private void Resource_AmountChanged(object sender, int amount)
        {
            // stock.text = resource.Amount.ToString();
        }

        private void OnEndEditAmount(string s)
        {
            currentAmount = int.Parse(s);
            if (currentAmount > resource.Amount)
            {
                currentAmount = resource.Amount;
                amountInput.text = currentAmount.ToString();
            }
        }

        #endregion EventMethods

        private void RemoveListeners()
        {
            if (previousResource == null) return;
            previousResource.AmountChanged -= Resource_AmountChanged;
            amountInput.onValueChanged.RemoveListener(OnEndEditAmount);
            minAmountButton.onClick.RemoveListener(MinAmount);
            maxAmountButton.onClick.RemoveListener(MaxAmount);
            sellButton.onClick.RemoveListener(ConfirmStocking);
        }
    }
}