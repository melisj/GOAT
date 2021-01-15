using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Goat.Pooling;
using System.Collections.Generic;
using Goat.Events;
using TMPro;
using DG.Tweening;
using UnityAtoms;
using UnityAtoms.BaseAtoms;

namespace Goat.UI
{
    public class SupplyWindow : BaseUIWindow, IAtomListener<bool>
    {
        [Title("Globals")]
        [SerializeField] private InputModeVariable inputMode;
        [SerializeField] private BoolEvent onDay;
        [SerializeField] private DeliveryResourceEvent deliveryIncoming;
        [SerializeField] private AnimateTabButton tabSwitcher;
        [Title("Buying")]
        [SerializeField] protected Button buyButton;
        [SerializeField, ShowIf("IsSupplyWindow")] private RectTransform noUnloadAreaWarning;
        [SerializeField, ShowIf("IsSupplyWindow")] private UnloadLocations unloadLocations;
        [SerializeField] private RectTransform buyButtonTransform;
        [SerializeField] private TMP_InputField buyAmount;
        [SerializeField] protected TextMeshProUGUI totalPrice;
        [SerializeField] private AudioCue errorSfx, confirmSfx;
        [Title("Content")]
        [SerializeField, ShowIf("IsSupplyWindow")] private RectTransform deliveryGrid;
        [SerializeField, ShowIf("IsSupplyWindow")] private GameObject deliveryPrefab;

        protected bool IsSupplyWindow => !(this.GetType().IsSubclassOf(typeof(SupplyWindow)));

        private Dictionary<Buyable, CellWithInventoryAmount> cellDict = new Dictionary<Buyable, CellWithInventoryAmount>();
        protected Buyable selectedBuyable;
        protected int currentAmount;
        private Sequence buyButtonAnimation;

        protected override void Awake()
        {
            base.Awake();
            Setup();
        }

        private void OnEnable()
        {
            tabSwitcher.OnTabSwitch += TabSwitcher_OnTabSwitch;
            if (onDay == null) return;
            onDay.RegisterListener(this);
        }

        protected virtual void TabSwitcher_OnTabSwitch(object sender, int e)
        {
            selectedBuyable = null;
            ResetAmount();
        }

        private void OnDisable()
        {
            tabSwitcher.OnTabSwitch -= TabSwitcher_OnTabSwitch;

            if (onDay == null) return;
            onDay.UnregisterListener(this);
        }

        protected virtual void Setup()
        {
            cellDict.Clear();
            buyAmount.onValueChanged.AddListener(OnBuyAmountChanged);
            buyButton.onClick.AddListener(Buy);
        }

        protected virtual void OnBuyAmountChanged(string s)
        {
            currentAmount = int.Parse(s);
            SetTotalPrice();
        }

        protected virtual void Buy()
        {
            if (!AnimateBuyButton(currentAmount > 0 && selectedBuyable != null && !selectedBuyable.CanBuy(currentAmount) && unloadLocations.Locations.Count > 0))
            {
                if (unloadLocations.Locations.Count <= 0)
                    noUnloadAreaWarning.gameObject.SetActive(true);
                return;
            }
            else if (unloadLocations.Locations.Count > 0)
            {
                noUnloadAreaWarning.gameObject.SetActive(false);
            }
            selectedBuyable.Buy(currentAmount, -1, true, false);
            SetupDeliveryCell(selectedBuyable, deliveryGrid, currentAmount);
        }

        /// <summary>
        /// Animated the buy button based on whether it is possible to buy
        /// </summary>
        /// <param name="validated"></param>
        /// <returns></returns>
        protected bool AnimateBuyButton(bool validated)
        {
            if (buyButtonAnimation != null)
            {
                buyButtonAnimation.Complete();
            }
            buyButtonAnimation = DOTween.Sequence();
            if (validated)
            {
                confirmSfx.PlayAudioCue();
                buyButtonAnimation.Append(buyButtonTransform.DOPunchScale(new Vector3(0.1f, 0.1f, 1), 0.2f, 15, 0.5f));
            }
            else
            {
                errorSfx.PlayAudioCue();
                buyButtonAnimation.Append(buyButtonTransform.DOShakeAnchorPos(0.3f, new Vector3(5, 0, 0), 90));
            }
            return validated;
        }

        public override void ShowUI()
        {
            base.ShowUI();
            inputMode.InputMode = InputMode.Select;
        }

        protected override void SetupCell(Buyable buyable, Transform grid, GridLayoutGroup currentLayoutGroup)
        {
            base.SetupCell(buyable, grid, currentLayoutGroup);
            UICell cellScript = cell.GetComponent<UICell>();
            cellScript.Setup(buyable);
            cellScript.OnClick(() => OnCellClick(buyable));
        }

        protected virtual void OnCellClick(Buyable buyable)
        {
            selectedBuyable = buyable;
            ResetAmount();
        }

        protected virtual void SetupDeliveryCell(Buyable buyable, RectTransform grid, int amount)
        {
            CellWithInventoryAmount deliveryScript;

            if (cellDict.ContainsKey(buyable))
            {
                cellDict.TryGetValue(buyable, out deliveryScript);
            }
            else
            {
                GameObject deliveryCell = PoolManager.Instance.GetFromPool(deliveryPrefab, grid.transform);
                deliveryScript = deliveryCell.GetComponent<CellWithInventoryAmount>();
                cellDict.Add(buyable, deliveryScript);
            }

            deliveryScript.Setup(buyable, deliveryScript.Amount + amount);
            deliveryIncoming.Raise(new DeliveryResource(buyable, amount));
        }

        protected virtual void SetTotalPrice()
        {
            if (selectedBuyable)
                totalPrice.text = (currentAmount * selectedBuyable.Price()).ToString("N0");
        }

        private void ResetAmount()
        {
            currentAmount = 0;
            buyAmount.SetTextWithoutNotify("amount...");
            SetTotalPrice();
        }

        /// <param name="item">IsDay</param>
        public void OnEventRaised(bool item)
        {
            if (item)
                ResetDictionary();
        }

        private void ResetDictionary()
        {
            var looper = cellDict.GetEnumerator();
            while (looper.MoveNext())
            {
                looper.Current.Value.Setup(looper.Current.Key, 0);
                PoolManager.Instance.ReturnToPool(looper.Current.Value.gameObject);
            }
        }
    }
}