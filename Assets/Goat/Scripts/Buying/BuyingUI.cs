using DG.Tweening;
using Goat.Farming;
using Goat.Pooling;
using Goat.Storage;
using Sirenix.OdinInspector;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Goat.UI
{
    public class BuyingUI : BaseBuyableUI
    {
        [Title("Grid Window")]
        [SerializeField] private GameObject deliveryPrefab;
        [SerializeField] private TMP_InputField amountField;
        [SerializeField] private RectTransform resourcesGrid;
        [SerializeField] private RectTransform deliveryGrid;
        [SerializeField] private RectTransform deliveryAmountIcon;
        [SerializeField] private TextMeshProUGUI deliveryAmount;
        [SerializeField] private RectTransform stationsGrid;
        [Title("Information window")]
        [SerializeField] private TextMeshProUGUI summary;
        [SerializeField] private TextMeshProUGUI price;
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI extraData;
        [SerializeField] private GameObject informationWindow;
        [Title("Selected")]
        [SerializeField] private Button buyButton;
        //
        private RectTransform buyButtonTransform;

        private int currentAmount;
        private int currentDeliveryAmount;

        private Sequence buyButtonAnimation;

        private GridLayoutGroup deliveryGroup;

        protected override void Awake()
        {
            base.Awake();
            amountField.onValueChanged.AddListener(OnAmountFieldChanged);
            deliveryAmountIcon.gameObject.SetActive(false);

            buyButtonInputTransform = buyButton.transform.parent.GetComponent<RectTransform>();
            buyButtonTransform = buyButton.GetComponent<RectTransform>();
            buyButtonInputTransform.localScale = new Vector3(0, 0, 1);
            buyButton.onClick.AddListener(Buy);
        }

        private void OnAmountFieldChanged(string s)
        {
            currentAmount = int.Parse(s);
            SetupInformationWindow(currentBuyable);
        }

        #region Buying

        private bool AnimateBuyButton(bool validated)
        {
            if (buyButtonAnimation != null)
            {
                buyButtonAnimation.Complete();
            }
            buyButtonAnimation = DOTween.Sequence();
            if (validated)
            {
                buyButtonAnimation.Append(buyButtonTransform.DOPunchScale(new Vector3(0.1f, 0.1f, 1), 0.2f, 15, 0.5f));
            }
            else
            {
                buyButtonAnimation.Append(buyButtonTransform.DOShakeAnchorPos(0.3f, new Vector3(5, 0, 0), 90));
            }
            return validated;
        }

        private void Buy()
        {
            //7 DIGITS
            //11 WIDTH TO 37 WIDTH
            //26/6 * (X DIGITS -1)
            bool enoughMoney = true;
            if (!AnimateBuyButton(currentAmount > 0 && enoughMoney)) return;
            if (currentBuyable.DeliveryTime > 0)
            {
                deliveryAmountIcon.gameObject.SetActive(true);
                float iconWidth = 11 + (26 / 6 * (currentDeliveryAmount.ToString().Length - 1));
                SetupDeliveryCell(currentBuyable, deliveryGrid, currentAmount);
                deliveryAmountIcon.sizeDelta = new Vector2(iconWidth, deliveryAmountIcon.sizeDelta.y);
                deliveryAmount.text = currentDeliveryAmount.ToString();
                if (!deliveryGroup)
                {
                    deliveryGroup = deliveryGrid.GetComponent<GridLayoutGroup>();
                }
                GridSizeFit(deliveryGrid, currentDeliveryAmount, deliveryGroup);
            }
            else
            {
                currentBuyable.Amount++;
            }
        }

        #endregion Buying

        #region Setup

        /// <summary>
        /// Fills grid
        /// </summary>
        protected override void SetupCellPositions()
        {
            base.SetupCellPositions();
            SetupGrid(resourcesGrid, "Resource");
            SetupGrid(stationsGrid, "Farming");
        }

        protected override void OnCellClick(Buyable buyable, int cellIndex)
        {
            base.OnCellClick(buyable, cellIndex);
            informationWindow.SetActive(true);
        }

        protected override GameObject SetupCell(Buyable buyable, Transform grid, GridLayoutGroup currentLayoutGroup, bool hasMostCells)
        {
            base.SetupCell(buyable, grid, currentLayoutGroup, hasMostCells);
            CellWithAmount cellScript = cell.GetComponent<CellWithAmount>();
            cellScript.Setup(buyable);
            return cell;
        }

        /// <summary>
        /// Creates grid cell for Delivery tab
        /// Delivery tab is different as it can be increased/decreased
        /// Which is why Pooling is used
        /// Immediately animates the progressbar of the cell
        /// </summary>
        /// <param name="buyable"></param>
        /// <param name="grid"></param>
        /// <param name="amount"></param>
        private void SetupDeliveryCell(Buyable buyable, RectTransform grid, int amount)
        {
            currentDeliveryAmount++;
            GameObject deliveryCell = PoolManager.Instance.GetFromPool(deliveryPrefab, Vector3.zero, Quaternion.identity, grid.transform);
            deliveryCell.transform.localScale = Vector3.one;

            DeliveryUI delivery = deliveryCell.GetComponent<DeliveryUI>();
            deliveryCell.name = buyable.name;
            delivery.Amount.text = "x" + amount.ToString();
            delivery.Name.text = deliveryCell.name;
            delivery.Image.sprite = buyable.Image;
            delivery.ProgressBar.DOFillAmount(1, buyable.DeliveryTime).OnComplete(() =>
            {
                buyable.Amount += amount;
                currentDeliveryAmount--;
                if (currentDeliveryAmount == 0)
                {
                    deliveryAmountIcon.gameObject.SetActive(false);
                }
                deliveryAmount.text = currentDeliveryAmount.ToString();
                delivery.ProgressBar.fillAmount = 0;
                PoolManager.Instance.ReturnToPool(deliveryCell);
            });
        }

        /// <summary>
        /// Setups the right side window, the information window
        /// </summary>
        /// <param name="buyable"></param>
        private void SetupInformationWindow(Buyable buyable)
        {
            if (!buyable) return;
            summary.text = buyable.Summary;
            price.text = (buyable.Price * currentAmount).ToString();
            image.sprite = buyable.Image;
            extraData.text = "";
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Delivery time: " + buyable.DeliveryTime.ToString());
            //Can't think of extra info for resources
            //if (buyable is Resource)
            //{
            //    Resource res = (Resource)buyable;
            //}

            if (buyable is FarmStation)
            {
                FarmStation station = (FarmStation)buyable;
                if (station.ResourceCost.Length > 0)
                {
                    for (int i = 0; i < station.ResourceCost.Length; i++)
                    {
                        ResourceCost resCost = station.ResourceCost[i];
                        stringBuilder.AppendLine(resCost.CostType.ToString() + "\tx" + resCost.Amount);
                    }
                }
                stringBuilder.AppendLine("Delivery type: " + station.FarmDeliverType);
                stringBuilder.AppendLine("Capacity: " + station.StorageCapacity.ToString());
                stringBuilder.AppendLine("Farm: " + station.ResourceFarm.ResourceType.ToString() + " x" + station.AmountPerSecond + "/s");
                if (station.FarmType == FarmType.OverTimeCost)
                {
                    stringBuilder.AppendLine("Production cost: " + station.CostPerSecond + "/s");
                }
            }

            extraData.text = stringBuilder.ToString();
            //if (buyable is Item)
            //{
            //    Item item = (Item)buyable;
            //}
        }

        #endregion Setup

        #region Selecting

        /// <summary>
        /// Deactivate current grid and activate new selected grid
        /// Lerp the position between old and new for animation
        /// </summary>
        /// <param name="tabIndex"></param>
        /// <param name="grid"></param>
        public override void SelectTab(int tabIndex, GameObject grid, RectTransform content, GridLayoutGroup layoutGroup)
        {
            base.SelectTab(tabIndex, grid, content, layoutGroup);
            tabSequence.Join(buyButtonInputTransform.DOScale(new Vector3(0, 0, 1), 0.2f));

            //Sneaky check for delivery tab
            if (tabIndex < tabPositions.Length - 1)
            {
                tabSequence.Join(selectedCellImage.DOFade(startAlphaCellImage, 0.1f));
            }

            ResetAmount();

            informationWindow.SetActive(false);
        }

        /// <summary>
        /// Select the current cell, and move the selection
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <param name="buyable"></param>
        protected override void SelectCell(int cellIndex, Buyable buyable)
        {
            base.SelectCell(cellIndex, buyable);
            cellSequence.Join(buyButtonInputTransform.DOScale(new Vector3(0, 0, 1), 0.2f));
            cellSequence.Append(buyButtonInputTransform.DOScale(new Vector3(1, 1, 1), 0.2f));

            ResetAmount();
            SetupInformationWindow(buyable);
        }

        private void ResetAmount()
        {
            currentAmount = 0;
            amountField.SetTextWithoutNotify("amount...");
        }

        #endregion Selecting
    }
}