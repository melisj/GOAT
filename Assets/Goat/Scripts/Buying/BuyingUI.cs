using Goat.Farming;
using Goat.Storage;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;
using System.Text;

namespace Goat.Buying
{
    public enum BuyType
    {
        Resources,
        FarmStations,
        Items
    }

    public class BuyingUI : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Data")] private ResourceDictionary resData;
        [SerializeField, FoldoutGroup("Data")] private FarmStationList stationData;
        //[SerializeField] private ItemDictionary itemData;
        [Title("Grid Window")]
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private GameObject gridWindow;
        [SerializeField] private RectTransform resourcesGrid;
        [SerializeField] private RectTransform stationsGrid;
        [Title("Tab Window")]
        [SerializeField] private GameObject leftTabWindow;
        [Title("Information window")]
        [SerializeField] private TextMeshProUGUI summary;
        [SerializeField] private TextMeshProUGUI price;
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI extraData;
        [SerializeField] private GameObject informationWindow;

        [Title("Selected")]
        [SerializeField] private BuyType selectedBuyType;
        [SerializeField] private RectTransform selectedTab;
        [SerializeField] private RectTransform selectedCell;

        private List<GameObject> resourceCells;
        private List<GameObject> stationCells;
        private List<GameObject> itemCells;
        private Buyable currentBuyable;

        private GameObject cell;
        private float[] tabPositions;
        private Vector2[] cellPositions;
        private int cellPosSize;
        private int currentTab;
        private int currentCell;
        private GameObject currentGrid;

        private void Awake()
        {
            resourceCells = new List<GameObject>();
            stationCells = new List<GameObject>();
            itemCells = new List<GameObject>();
            SetupCellPositions();
            SetupTabPositions();
        }

        private void SetupTabPositions()
        {
            tabPositions = new float[leftTabWindow.transform.childCount];
            for (int i = 0; i < leftTabWindow.transform.childCount; i++)
            {
                RectTransform tabChild = leftTabWindow.transform.GetChild(i).GetComponent<RectTransform>();

                tabPositions[i] = tabChild.anchoredPosition.y;
                RectTransform gridChild = gridWindow.transform.GetChild(i).GetComponent<RectTransform>();
                SetupPairs(i, tabChild, gridChild);
            }
            currentGrid = gridWindow.transform.GetChild(0).gameObject;
        }

        private void SetupPairs(int index, Transform tab, Transform grid)
        {
            Button tabButton = tab.GetComponent<Button>();
            tabButton.onClick.AddListener(() => SelectTab(index, grid.gameObject));
        }

        /// <summary>
        /// Fills grid
        /// </summary>
        private void SetupCellPositions()
        {
            currentCell = 0;
            bool mostCells = resData.Resources.Count > stationData.FarmStations.Count;
            SetupResources(resourcesGrid, mostCells);
            SetupStations(stationsGrid, mostCells);
        }

        /// <summary>
        /// Deactivate current grid and activate new selected grid
        /// Lerp the position between old and new for animation
        /// </summary>
        /// <param name="tabIndex"></param>
        /// <param name="grid"></param>
        public void SelectTab(int tabIndex, GameObject grid)
        {
            float oldPos = tabPositions[currentTab];
            if (tabIndex >= tabPositions.Length)
            {
                tabIndex = 0;
            }
            currentGrid.SetActive(false);
            grid.SetActive(true);
            currentGrid = grid;
            currentTab = tabIndex;
            selectedTab.anchoredPosition = new Vector2(selectedTab.anchoredPosition.x, Mathf.Lerp(oldPos, tabPositions[currentTab], 0.1f));
        }

        /// <summary>
        /// Select the current cell, and move the selection
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <param name="buyable"></param>
        private void SelectCell(int cellIndex, Buyable buyable)
        {
            Vector2 oldPos = cellPositions[cellIndex];
            if (cellIndex >= cellPositions.Length)
            {
                cellIndex = 0;
            }

            currentCell = cellIndex;

            selectedCell.anchoredPosition = Vector2.Lerp(oldPos, cellPositions[cellIndex], 0.1f);
            currentBuyable = buyable;
            SetupInformationWindow(buyable);
        }

        /// <summary>
        /// Creates grid cell based on buyable type
        /// </summary>
        /// <param name="buyable"></param>
        /// <param name="tab"></param>
        /// <param name="hasMostCells"></param>
        /// <returns></returns>
        private GameObject SetupCell(Buyable buyable, Transform grid, bool hasMostCells)
        {
            cell = Instantiate(cellPrefab, grid);
            Button imageButton = cell.GetComponent<Button>();
            Image image = cell.GetComponentInChildren<Image>();
            TextMeshProUGUI name = cell.GetComponentInChildren<TextMeshProUGUI>();
            imageButton.onClick.AddListener(() => informationWindow.SetActive(true));
            imageButton.onClick.AddListener(() => SelectCell(currentCell, buyable));
            cell.name = buyable.name;
            name.text = buyable.name;
            image.sprite = buyable.Image;

            if (hasMostCells)
            {
                if (cellPositions == null)
                {
                    cellPositions = new Vector2[cellPosSize];
                }
                cellPositions[currentCell] = cell.transform.position;
            }
            return cell;
        }

        private void SetupResources(Transform grid, bool hasMostCells)
        {
            cellPosSize = hasMostCells ? resData.Resources.Count : 0;
            var enumerator = resData.Resources.GetEnumerator();
            while (enumerator.MoveNext())
            {
                resourceCells.Add(SetupCell(enumerator.Current.Value, grid, hasMostCells));
                currentCell++;
            }
            currentCell = 0;
        }

        private void SetupStations(Transform grid, bool hasMostCells)
        {
            cellPosSize = hasMostCells ? stationData.FarmStations.Count : 0;
            for (int i = 0; i < stationData.FarmStations.Count; i++, currentCell++)
            {
                FarmStationSettings stationSettings = stationData.FarmStations[i];
                stationCells.Add(SetupCell(stationSettings, grid, hasMostCells));
            }
            currentCell = 0;
        }

        private void SetupItems()
        {
            //foreach (KeyValuePair<ItemType, Item> res in resData.Resources)
            //{
            //    //  SetupCell(res.Value);
            //}
        }

        private void SetupInformationWindow(Buyable buyable)
        {
            summary.text = buyable.Summary;
            price.text = buyable.Price.ToString();
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
    }
}