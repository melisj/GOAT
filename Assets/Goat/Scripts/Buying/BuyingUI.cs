using DG.Tweening;
using Goat.Farming;
using Goat.Pooling;
using Goat.Storage;
using Sirenix.OdinInspector;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Goat.Buying
{
    public class BuyingUI : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Data")] private ResourceDictionary resData;
        [SerializeField, FoldoutGroup("Data")] private FarmStationList stationData;
        //[SerializeField] private ItemDictionary itemData;
        [Title("Grid Window")]
        [SerializeField] private TMP_InputField searchField;
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private GameObject deliveryPrefab;
        [SerializeField] private TMP_InputField amountField;
        [SerializeField] private GameObject gridWindow;
        [SerializeField] private RectTransform resourcesGrid;
        [SerializeField] private RectTransform deliveryGrid;
        [SerializeField] private RectTransform deliveryAmountIcon;
        [SerializeField] private TextMeshProUGUI deliveryAmount;
        [SerializeField] private RectTransform stationsGrid;
        [Title("Tab Window")]
        [SerializeField] private GameObject leftTabWindow;
        [SerializeField] private TextMeshProUGUI textWindow;
        [Title("Information window")]
        [SerializeField] private TextMeshProUGUI summary;
        [SerializeField] private TextMeshProUGUI price;
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI extraData;
        [SerializeField] private GameObject informationWindow;

        [Title("Selected")]
        [SerializeField] private ScrollRect scrollingRect;
        [SerializeField] private VerticalLayoutGroup verticalGroup;

        [SerializeField] private Button buyButton;
        [SerializeField] private RectTransform selectedTab;
        [SerializeField] private RectTransform tabArrow;
        [SerializeField] private RectTransform selectedCell;

        private Buyable currentBuyable;

        private float[] tabPositions;
        private Vector2[] cellPositions;

        private GameObject cell;
        private int cellPosSize;
        private int currentTab;
        private int currentCell;

        private GameObject currentGrid;
        private RectTransform buyButtonInputTransform;
        private RectTransform buyButtonTransform;

        private Image selectedCellImage;
        private float startAlphaCellImage;
        private Vector2 cellStartPos;
        private int currentAmount;
        private int currentDeliveryAmount;

        private Sequence tabSequence;
        private Sequence cellSequence;
        private Sequence buyButtonAnimation;

        private GridLayoutGroup deliveryGroup;
        private GridLayoutGroup currentGroup;
        private GridLayoutGroup resourcesGridGroup;
        private GridLayoutGroup stationsGridGroup;
        private GridLayoutGroup itemsGridGroup;

        private void Awake()
        {
            cellStartPos = selectedCell.anchoredPosition;

            amountField.onValueChanged.AddListener(OnAmountFieldChanged);
            deliveryAmountIcon.gameObject.SetActive(false);
            int aCount = resData.Resources.Count;
            SetupCellPositions();
            SetupTabPositions();

            selectedCellImage = selectedCell.GetChild(0).GetComponent<Image>();
            startAlphaCellImage = selectedCellImage.color.a;

            buyButtonInputTransform = buyButton.transform.parent.GetComponent<RectTransform>();
            buyButtonTransform = buyButton.GetComponent<RectTransform>();
            buyButtonInputTransform.localScale = new Vector3(0, 0, 1);
            buyButton.onClick.AddListener(Buy);

            searchField.onValueChanged.AddListener(SearchFor);
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
        /// Fills the tabPositions array
        /// And GetsAllComponents needed
        /// </summary>
        private void SetupTabPositions()
        {
            tabPositions = new float[leftTabWindow.transform.childCount];
            verticalGroup.enabled = false;
            for (int i = 0; i < leftTabWindow.transform.childCount; i++)
            {
                RectTransform tabChild = leftTabWindow.transform.GetChild(i).GetComponent<RectTransform>();

                tabPositions[i] = tabChild.anchoredPosition.y;
                RectTransform gridChild = gridWindow.transform.GetChild(i).GetComponent<RectTransform>();
                GridLayoutGroup layoutGroup = gridWindow.transform.GetChild(i).GetComponent<GridLayoutGroup>();

                SetupPairs(i, tabChild, gridChild, gridChild, layoutGroup);
            }
            verticalGroup.enabled = true;
            currentGrid = gridWindow.transform.GetChild(0).gameObject;
        }

        /// <summary>
        /// Setups the OnClick event to push needed data to selectTab
        /// </summary>
        private void SetupPairs(int index, Transform tab, Transform grid, RectTransform content, GridLayoutGroup layoutGroup)
        {
            Button tabButton = tab.GetComponent<Button>();
            tabButton.onClick.AddListener(() => SelectTab(index, grid.gameObject, content, layoutGroup));
        }

        /// <summary>
        /// Fills grid
        /// </summary>
        private void SetupCellPositions()
        {
            currentCell = 0;
            Debug.Log("Line 192");
            bool mostCells = resData.Resources.Count > stationData.FarmStations.Count;
            SetupResources(resourcesGrid, mostCells);
            SetupStations(stationsGrid, !mostCells);
        }

        /// <summary>
        /// Creates grid cell based on buyable type
        /// </summary>
        /// <param name="buyable"></param>
        /// <param name="tab"></param>
        /// <param name="hasMostCells"></param>
        /// <returns></returns>
        private GameObject SetupCell(Buyable buyable, Transform grid, GridLayoutGroup currentLayoutGroup, bool hasMostCells)
        {
            currentLayoutGroup.enabled = false;
            cell = Instantiate(cellPrefab, grid);

            Button imageButton = cell.GetComponent<Button>();
            Image image = cell.GetComponentInChildren<Image>();
            TextMeshProUGUI name = cell.GetComponentInChildren<TextMeshProUGUI>();

            int cellIndex = currentCell;
            imageButton.onClick.AddListener(() => informationWindow.SetActive(true));
            imageButton.onClick.AddListener(() => SelectCell(cellIndex, buyable));

            cell.name = buyable.name;
            name.text = buyable.name;
            image.sprite = buyable.Image;

            //Enabling the layout to make Unity update the positions, force updating it to make the positions available now
            currentLayoutGroup.enabled = true;
            Canvas.ForceUpdateCanvases();

            if (hasMostCells)
            {
                if (cellPositions == null || cellPositions.Length == 0)
                {
                    cellPositions = new Vector2[cellPosSize];
                }
                //Disabling it so it is readable
                currentLayoutGroup.enabled = false;
                Canvas.ForceUpdateCanvases();

                cellPositions[currentCell] = cell.GetComponent<RectTransform>().anchoredPosition;

                currentLayoutGroup.enabled = true;
                Canvas.ForceUpdateCanvases();
            }

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
        /// Creates the whole grid of Resources
        /// Each grid has the same layout excl. delivery
        /// So each cell has same position
        /// MostCells is used to only make an array of positions of the biggest grid
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="hasMostCells"></param>
        private void SetupResources(Transform grid, bool hasMostCells)
        {
            resourcesGridGroup = grid.GetComponent<GridLayoutGroup>();
            cellPosSize = hasMostCells ? resData.Resources.Count : 0;
            var enumerator = resData.Resources.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SetupCell(enumerator.Current.Value, grid, resourcesGridGroup, hasMostCells);
                currentCell++;
            }

            GridSizeFit(resourcesGrid, resData.Resources.Count, resourcesGridGroup);

            currentCell = 0;
        }

        /// <summary>
        /// Creates the whole grid of stations
        /// Similar to SetupResources
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="hasMostCells"></param>
        private void SetupStations(Transform grid, bool hasMostCells)
        {
            cellPosSize = hasMostCells ? stationData.FarmStations.Count : 0;
            stationsGridGroup = grid.GetComponent<GridLayoutGroup>();

            for (int i = 0; i < stationData.FarmStations.Count; i++, currentCell++)
            {
                FarmStationSettings stationSettings = stationData.FarmStations[i];
                SetupCell(stationSettings, grid, stationsGridGroup, hasMostCells);
            }

            GridSizeFit(stationsGrid, stationData.FarmStations.Count, stationsGridGroup);
            currentCell = 0;
        }

        /// <summary>
        /// Items aren't made yet
        /// </summary>
        private void SetupItems()
        {
            //foreach (KeyValuePair<ItemType, Item> res in resData.Resources)
            //{
            //    //  SetupCell(res.Value);
            //}
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

            //Can't think of extra info for resources
            //if (buyable is Resource)
            //{
            //    Resource res = (Resource)buyable;
            //}

            if (buyable is FarmStationSettings)
            {
                FarmStationSettings station = (FarmStationSettings)buyable;
                StringBuilder stringBuilder = new StringBuilder();
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
                stringBuilder.AppendLine("Farm: " + station.ResourceFarm.ResourceType.ToString() + "\tx" + station.AmountPerSecond + "/s");
                if (station.FarmType == FarmType.OverTimeCost)
                {
                    stringBuilder.AppendLine("Production cost: " + station.CostPerSecond + "/s");
                }
                extraData.text = stringBuilder.ToString();
            }

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
        public void SelectTab(int tabIndex, GameObject grid, RectTransform content, GridLayoutGroup layoutGroup)
        {
            float oldPos = tabPositions[currentTab];
            if (tabIndex >= tabPositions.Length)
            {
                tabIndex = 0;
            }

            currentGrid.SetActive(false);
            grid.SetActive(true);
            currentGrid = grid;
            currentGroup = layoutGroup;
            currentTab = tabIndex;
            scrollingRect.content = content;
            searchField.text = "";
            if (tabSequence != null)
            {
                tabSequence.Complete();
            }

            ResetAmount();

            textWindow.text = "   1. " + currentGrid.name;

            tabSequence = DOTween.Sequence();
            tabSequence.Append(tabArrow.DOScaleX(0, 0.1f));
            tabSequence.Join(buyButtonInputTransform.DOScale(new Vector3(0, 0, 1), 0.1f));
            tabSequence.Join(selectedCellImage.DOFade(0, 0.1f));
            tabSequence.Append(selectedTab.DOAnchorPosY(tabPositions[currentTab], 0.1f));
            tabSequence.Join(selectedCell.DOAnchorPos(cellStartPos, 0.1f));
            tabSequence.Join(tabArrow.DOScaleX(1, 0.2f));

            //Sneaky check for delivery tab
            if (tabIndex < tabPositions.Length - 1)
            {
                tabSequence.Join(selectedCellImage.DOFade(startAlphaCellImage, 0.1f));
            }

            informationWindow.SetActive(false);
        }

        /// <summary>
        /// Select the current cell, and move the selection
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <param name="buyable"></param>
        private void SelectCell(int cellIndex, Buyable buyable)
        {
            if (cellIndex >= cellPositions.Length)
            {
                cellIndex = 0;
            }

            currentCell = cellIndex;

            if (cellSequence != null)
            {
                cellSequence.Complete();
            }
            ResetAmount();
            cellSequence = DOTween.Sequence();
            cellSequence.Append(buyButtonInputTransform.DOScale(new Vector3(0, 0, 1), 0.1f));
            cellSequence.Append(selectedCell.DOAnchorPos(cellPositions[cellIndex], 0.1f));
            cellSequence.Append(buyButtonInputTransform.DOScale(new Vector3(1, 1, 1), 0.2f));

            currentBuyable = buyable;
            SetupInformationWindow(buyable);
        }

        private void ResetAmount()
        {
            currentAmount = 0;
            amountField.SetTextWithoutNotify("amount...");
        }

        #endregion Selecting

        /// <summary>
        /// Toggles cells in the currentgrid based on the input of the searchbar
        /// </summary>
        /// <param name="s"></param>
        private void SearchFor(string s)
        {
            int amountChildrenActivated = 0;
            if (tabSequence != null)
            {
                tabSequence.Complete();
            }
            tabSequence = DOTween.Sequence();
            tabSequence.Append(buyButtonInputTransform.DOScale(new Vector3(0, 0, 1), 0.1f));
            tabSequence.Join(selectedCellImage.DOFade(0, 0.1f));
            tabSequence.Join(selectedCell.DOAnchorPos(cellStartPos, 0.1f));

            if (currentTab < tabPositions.Length - 1)
            {
                tabSequence.Append(selectedCellImage.DOFade(startAlphaCellImage, 0.1f));
            }

            for (int i = 0; i < currentGrid.transform.childCount; i++)
            {
                GameObject child = currentGrid.transform.GetChild(i).gameObject;
                bool contains = child.name.IndexOf(s, System.StringComparison.OrdinalIgnoreCase) >= 0;
                if (contains)
                {
                    amountChildrenActivated++;
                }
                child.SetActive(contains);
            }
            GridSizeFit(scrollingRect.content, amountChildrenActivated, currentGroup);
        }

        /// <summary>
        /// Adjusts the size of the grid based on amount cells
        /// (Basically a content size fitter, but one that doesn't mess up the UI)
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="count"></param>
        /// <param name="gridLayout"></param>
        private void GridSizeFit(RectTransform grid, int count, GridLayoutGroup gridLayout = null)
        {
            if (count > 16)
            {
                if (!gridLayout)
                {
                    gridLayout = grid.GetComponent<GridLayoutGroup>();
                }

                int sizeChange = Mathf.CeilToInt((count - 16f) / 4f);
                float bottom = ((gridLayout.spacing.y + gridLayout.cellSize.y) * sizeChange);

                grid.offsetMax = new Vector2(0, 0);
                grid.offsetMin = new Vector2(0, -bottom);
            }
        }
    }
}