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
    public class StockingUI : MonoBehaviour
    {
        [Title("Resource UI")]
        [SerializeField] private Resource resource;
        [SerializeField] private GameObject stockingUI;
        [SerializeField] private TextMeshProUGUI stockButtonText;
        [SerializeField] private TextMeshProUGUI resourceName;
        //[SerializeField] private TextMeshProUGUI stock;
        //[SerializeField] private Image resourceImage;

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

        private void OnResourceChanged(Inventory from, Inventory to)
        {
            RemoveListeners();
            previousResource = resource;
            SetupUI(from, to);
        }

        private void SetupUI(Inventory from, Inventory to)
        {
            SetupResourceUI();
        }

        private void SetupResourceUI()
        {
            if (resourceName)
                resourceName.text = resource.name.ToString();
            resource.AmountChanged += Resource_AmountChanged;
        }

        #region EventMethods

        private void Resource_AmountChanged(object sender, int amount)
        {
            // stock.text = resource.Amount.ToString();
        }

        #endregion EventMethods

        private void RemoveListeners()
        {
            if (previousResource == null) return;
            previousResource.AmountChanged -= Resource_AmountChanged;
        }
    }
}