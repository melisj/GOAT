using Goat.Events;
using Goat.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Goat.Player;
using DG.Tweening;
using Sirenix.OdinInspector;

// Original Author: Stanley

namespace Goat.Grid.Interactions.UI
{
    /// <summary>
    /// Script for managing the UI of stocking storage units in the game
    /// </summary>
    public class StockingUI : EventListenerKeyCodeModeEvent
    {
        [Title("Resource UI")]
        [SerializeField] private Resource resource;
        [SerializeField] private GameObject stockingUI;
        [SerializeField] private TextMeshProUGUI stockButtonText;
        [SerializeField] private TextMeshProUGUI resourceName;
        //[SerializeField] private TextMeshProUGUI stock;
        //[SerializeField] private Image resourceImage;
        [Title("Inputs")]
        [SerializeField] private TMP_InputField amountInput;
        [Title("Buttons")]
        [SerializeField] private Button minAmountButton;
        [SerializeField] private Button maxAmountButton;
        [SerializeField] private Button sellButton;
        [Title("Audio")]
        [SerializeField] private AudioCue confirmSfx, errorSfx;
        [Title("References")]
        [SerializeField] private InputModeVariable currentMode;
        [SerializeField] private InteractablesInfo interactableInfo;
        [SerializeField] private PlayerInventory playerInventory;

        public GameObject StockingUIElement => stockingUI;
        public TextMeshProUGUI StockButtonText => stockButtonText;
        private int currentAmount;

        private int CurrentAmount
        {
            get => currentAmount;
            set
            {
                currentAmount = value;
                amountInput.text = currentAmount.ToString();
            }
        }

        private Resource previousResource;
        private Sequence buyButtonAnimation;
        private RectTransform sellButtonTransform;

        public Resource Resource
        {
            get => resource;
            private set => resource = value;
        }

        public void ChangeResource(Resource resource, Inventory from, Inventory to)
        {
            Resource = resource;
            OnResourceChanged(from, to);
        }

        public void ChangeResource(Resource resource, Inventory from, StorageInteractable toInteractable)
        {
            Resource = resource;
            if (toInteractable)
                OnResourceChanged(from, toInteractable.Inventory);
        }

        private void Awake()
        {
            previousResource = resource;
            SetupUI(null, null);
        }

        public override void OnEventRaised(KeyCodeMode value)
        {
            KeyCode code = KeyCode.None;
            KeyMode mode = KeyMode.None;

            value.Deconstruct(out code, out mode);
            OnInput(code, mode);
        }

        private void Update()
        {
            if (currentMode.InputMode == InputMode.Select &&
                stockingUI.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.KeypadEnter) | Input.GetKeyDown(KeyCode.Return))
                {
                    sellButton.onClick.Invoke();
                }
            }
        }

        private void OnInput(KeyCode code, KeyMode keyMode)
        {
            if (keyMode == KeyMode.Down &&
                currentMode.InputMode == InputMode.Select &&
                stockingUI.activeInHierarchy)
            {
                if (code == KeyCode.KeypadEnter | code == KeyCode.Return)
                {
                    sellButton.onClick.Invoke();
                }

                if (code == KeyCode.F)
                {
                    maxAmountButton.onClick.Invoke();
                }

                if (code == KeyCode.E)
                {
                    minAmountButton.onClick.Invoke();
                }
            }
        }

        private void OnResourceChanged(Inventory from, Inventory to)
        {
            RemoveListeners();
            previousResource = resource;
            SetupUI(from, to);
        }

        private void SetupUI(Inventory from, Inventory to)
        {
            SetupResourceUI();
            SetupInputUI(from, to);
            SetButtonUI(from, to);
        }

        private void SetupResourceUI()
        {
            if (resourceName)
                resourceName.text = resource.name.ToString();
            resource.AmountChanged += Resource_AmountChanged;
        }

        private void SetupInputUI(Inventory from, Inventory to)
        {
            amountInput.onValueChanged.AddListener((s) => OnEndEditAmount(s, from));
            MinAmount(from, to);
        }

        private void SetButtonUI(Inventory from, Inventory to)
        {
            if (minAmountButton)
                minAmountButton.onClick.AddListener(() => MinAmount(from, to));
            if (maxAmountButton)
                maxAmountButton.onClick.AddListener(() => MaxAmount(from, to));
            sellButton.onClick.AddListener(() => ConfirmStocking(from, to));
        }

        #region EventMethods

        private void MaxAmount(Inventory from, Inventory to)
        {
            from.Items.TryGetValue(resource, out int amount);
            amountInput.text = Mathf.Min(amount, to.SpaceLeft).ToString();
        }

        private void MinAmount(Inventory from, Inventory to)
        {
            if (from != null)
            {
                from.Items.TryGetValue(resource, out int amount);
                int minimalValue = Mathf.Min(amount, to.SpaceLeft);
                amountInput.text = minimalValue > 0 ? "1" : minimalValue.ToString();
            }
        }

        private void ConfirmStocking(Inventory from, Inventory to)
        {
            if (!AnimateBuyButton(from != null)) return;
            from.Items.TryGetValue(resource, out int fromResourceAmount);

            if (!AnimateBuyButton((fromResourceAmount - CurrentAmount) >= 0))
            {
                CurrentAmount = (fromResourceAmount - CurrentAmount) < 0 ? fromResourceAmount : CurrentAmount;
                return;
            }
            CurrentAmount = (fromResourceAmount - CurrentAmount) < 0 ? fromResourceAmount : CurrentAmount;

            to.Add(resource, CurrentAmount, out int actualStoredAmount);
            from.Remove(resource, actualStoredAmount, out int removedAmount);

            //stockingUI.SetActive(false);
        }

        private void Resource_AmountChanged(object sender, int amount)
        {
            // stock.text = resource.Amount.ToString();
        }

        private void OnEndEditAmount(string s, Inventory from)
        {
            CurrentAmount = int.Parse(s);
            from.Items.TryGetValue(resource, out int resourceAmount);
            if (CurrentAmount > resourceAmount)
            {
                CurrentAmount = resourceAmount;
                amountInput.text = CurrentAmount.ToString();
            }
        }

        #endregion EventMethods

        /// <summary>
        /// Animated the buy button based on whether it is possible to buy
        /// </summary>
        /// <param name="validated"></param>
        /// <returns></returns>
        protected bool AnimateBuyButton(bool validated)
        {
            if (sellButtonTransform == null)
            {
                sellButtonTransform = sellButton.GetComponent<RectTransform>();
            }

            if (buyButtonAnimation != null)
            {
                buyButtonAnimation.Complete();
            }
            buyButtonAnimation = DOTween.Sequence();
            if (validated)
            {
                confirmSfx.PlayAudioCue();
                buyButtonAnimation.Append(sellButtonTransform.DOPunchScale(new Vector3(0.1f, 0.1f, 1), 0.2f, 15, 0.5f));
            }
            else
            {
                errorSfx.PlayAudioCue();
                buyButtonAnimation.Append(sellButtonTransform.DOShakeAnchorPos(0.3f, new Vector3(5, 0, 0), 90));
            }
            return validated;
        }

        private void RemoveListeners()
        {
            if (previousResource == null) return;
            previousResource.AmountChanged -= Resource_AmountChanged;
            amountInput.onValueChanged.RemoveAllListeners();
            //minAmountButton.onClick.RemoveAllListeners();
            //maxAmountButton.onClick.RemoveAllListeners();
            sellButton.onClick.RemoveAllListeners();
        }
    }
}