using DG.Tweening;
using Goat.Grid.UI;
using Sirenix.OdinInspector;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

namespace Goat.UI
{
    public class BaseBuyableUI : BasicGridUIElement
    {
        [Title("Event")]
        [SerializeField, FoldoutGroup("Base")] protected BoolVariable inputFieldSelected;

        [Title("Base Grid Window")]
        [SerializeField, FoldoutGroup("Base")] protected TMP_InputField searchField;
        [SerializeField, FoldoutGroup("Base")] protected GameObject gridWindow;
        [SerializeField, FoldoutGroup("Base")] protected GameObject cellPrefab;
        [Title("Base Tab Window")]
        [SerializeField, FoldoutGroup("Base")] protected GameObject leftTabWindow;
        [SerializeField, FoldoutGroup("Base")] protected TextMeshProUGUI textWindow;

        [Title("Base Selected")]
        [SerializeField, FoldoutGroup("Base")] protected ScrollRect scrollingRect;
        [SerializeField, FoldoutGroup("Base")] protected VerticalLayoutGroup verticalGroup;

        [SerializeField, FoldoutGroup("Base")] protected RectTransform selectedTab;
        [SerializeField, FoldoutGroup("Base")] protected RectTransform tabArrow;
        [SerializeField, FoldoutGroup("Base")] protected RectTransform selectedCell;

        protected Buyable currentBuyable;

        [SerializeField] protected float[] tabPositions;
        [SerializeField] protected Vector2[] cellPositions;

        protected GameObject cell;
        protected int currentTab;
        protected int currentCell;

        protected GameObject currentGrid;
        protected RectTransform buyButtonInputTransform;

        protected Image selectedCellImage;
        protected float startAlphaCellImage;
        protected Vector2 cellStartPos;

        protected Sequence tabSequence;
        protected Sequence cellSequence;

        protected GridLayoutGroup currentGroup;

        protected virtual void Awake()
        {
            cellStartPos = selectedCell.anchoredPosition;

            SetupCellPositions();
            SetupTabPositions();

            selectedCellImage = selectedCell.GetChild(0).GetComponent<Image>();
            startAlphaCellImage = selectedCellImage.color.a;
            searchField.onSelect.AddListener((string s) => ToggleInput(true));
            searchField.onDeselect.AddListener((string s) => ToggleInput(false));

            searchField.onValueChanged.AddListener(SearchFor);
        }

        #region Setup

        /// <summary>
        /// Fills the tabPositions array
        /// And GetsAllComponents needed
        /// </summary>
        private void SetupTabPositions()
        {
            tabPositions = new float[leftTabWindow.transform.childCount];
            verticalGroup.enabled = true;
            Canvas.ForceUpdateCanvases();

            for (int i = 0; i < leftTabWindow.transform.childCount; i++)
            {
                RectTransform tabChild = leftTabWindow.transform.GetChild(i).GetComponent<RectTransform>();
                verticalGroup.enabled = false;
                Canvas.ForceUpdateCanvases();
                tabPositions[i] = tabChild.anchoredPosition.y;
                RectTransform gridChild = gridWindow.transform.GetChild(i).GetComponent<RectTransform>();
                GridLayoutGroup layoutGroup = gridWindow.transform.GetChild(i).GetComponent<GridLayoutGroup>();

                SetupPairs(i, tabChild, gridChild, gridChild, layoutGroup);
            }
            verticalGroup.enabled = true;
            Canvas.ForceUpdateCanvases();

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
        protected virtual void SetupCellPositions()
        {
            currentCell = 0;
        }

        /// <summary>
        /// Creates grid cell based on buyable type
        /// </summary>
        /// <param name="buyable"></param>
        /// <param name="tab"></param>
        /// <param name="hasMostCells"></param>
        /// <returns></returns>
        protected virtual GameObject SetupCell(Buyable buyable, Transform grid, GridLayoutGroup currentLayoutGroup, bool hasMostCells)
        {
            currentLayoutGroup.enabled = false;
            cell = Instantiate(cellPrefab, grid);

            Button imageButton = cell.GetComponent<Button>();
            Image image = cell.GetComponentInChildren<Image>();
            TextMeshProUGUI name = cell.GetComponentInChildren<TextMeshProUGUI>();
            int cellIndex = currentCell;
            imageButton.onClick.AddListener(() => OnCellClick(buyable, cellIndex));

            cell.name = buyable.name;
            name.text = buyable.name;
            image.sprite = buyable.Image;

            //Enabling the layout to make Unity update the positions, force updating it to make the positions available now
            currentLayoutGroup.enabled = true;
            Canvas.ForceUpdateCanvases();

            if (hasMostCells)
            {
                //Disabling it so it is readable
                currentLayoutGroup.enabled = false;
                Canvas.ForceUpdateCanvases();

                cellPositions[currentCell] = cell.GetComponent<RectTransform>().anchoredPosition;

                currentLayoutGroup.enabled = true;
                Canvas.ForceUpdateCanvases();
            }

            return cell;
        }

        protected virtual void OnCellClick(Buyable buyable, int cellIndex)
        {
            SelectCell(cellIndex, buyable);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="resourcesPath">Path to folder beyond Resources e.g: "Resource" "Farming"</param>
        protected void SetupGrid(RectTransform grid, string resourcesPath)
        {
            GridLayoutGroup gridGroup = grid.GetComponent<GridLayoutGroup>();
            Buyable[] buyableCells = Resources.LoadAll<Buyable>(resourcesPath);

            bool mostCells = cellPositions == null || cellPositions.Length == 0 || cellPositions.Length < buyableCells.Length;
            if (mostCells)
            {
                cellPositions = new Vector2[buyableCells.Length];
            }

            for (int i = 0; i < buyableCells.Length; i++, currentCell++)
            {
                Buyable buyableCell = buyableCells[i];
                SetupCell(buyableCell, grid, gridGroup, mostCells);
            }

            GridSizeFit(grid, buyableCells.Length, gridGroup);

            currentCell = 0;
        }

        ///

        #endregion Setup

        #region Selecting

        /// <summary>
        /// Deactivate current grid and activate new selected grid
        /// Lerp the position between old and new for animation
        /// </summary>
        /// <param name="tabIndex"></param>
        /// <param name="grid"></param>
        public virtual void SelectTab(int tabIndex, GameObject grid, RectTransform content, GridLayoutGroup layoutGroup)
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

            textWindow.text = "   1. " + currentGrid.name;

            tabSequence = DOTween.Sequence();
            tabSequence.Append(tabArrow.DOScaleX(0, 0.1f));
            tabSequence.Join(selectedCellImage.DOFade(0, 0.1f));
            tabSequence.Append(selectedTab.DOAnchorPosY(tabPositions[currentTab], 0.1f));
            tabSequence.Join(selectedCell.DOAnchorPos(cellStartPos, 0.1f));
            tabSequence.Join(tabArrow.DOScaleX(1, 0.2f));
        }

        /// <summary>
        /// Select the current cell, and move the selection
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <param name="buyable"></param>
        protected virtual void SelectCell(int cellIndex, Buyable buyable)
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
            currentBuyable = buyable;
            cellSequence = DOTween.Sequence();
            cellSequence.Append(selectedCell.DOAnchorPos(cellPositions[cellIndex], 0.1f));
        }

        #endregion Selecting

        protected void ToggleInput(bool enable)
        {
            inputFieldSelected.Value = enable;
        }

        /// <summary>
        /// Toggles cells in the currentgrid based on the input of the searchbar
        /// </summary>
        /// <param name="s"></param>
        protected void SearchFor(string s)
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
        protected void GridSizeFit(RectTransform grid, int count, GridLayoutGroup gridLayout = null)
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