using Goat.Grid.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Goat.UI
{
    using Grid = Grid.Grid;

    public class BuildingUI : BaseBuyableUI
    {
        [SerializeField] private RectTransform floorGrid;
        [SerializeField] private RectTransform wallsGrid;
        [SerializeField] private RectTransform furnitureGrid;
        [SerializeField] private RectTransform farmsGrid;
        [SerializeField] private RectTransform tubesGrid;
        [SerializeField] private Grid grid;
        [SerializeField] private GridUIInfo gridUIInfo;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void SetupCellPositions()
        {
            base.SetupCellPositions();
            SetupGrid(floorGrid, "Floors");
            SetupGrid(wallsGrid, "Walls");
            SetupGrid(furnitureGrid, "Furniture");
            SetupGrid(farmsGrid, "Farming");
            SetupGrid(tubesGrid, "Tubes");
        }

        protected override void SelectCell(int cellIndex, Buyable buyable)
        {
            if (cellIndex >= cellPositions.Length)
            {
                cellIndex = 0;
            }

            currentCell = cellIndex;
            if (buyable.Amount > 0)
            {
                //  grid.interactionMode = Goat.Grid.SelectionMode.Edit;
                grid.ChangePreviewObject((Placeable)buyable);
                gridUIInfo.CurrentUIElement = GridUIElement.None;
                //gameObject.SetActive(false);
            }
        }

        protected override GameObject SetupCell(Buyable buyable, Transform grid, GridLayoutGroup currentLayoutGroup, bool hasMostCells)
        {
            base.SetupCell(buyable, grid, currentLayoutGroup, hasMostCells);
            CellWithAmount cellScript = cell.GetComponent<CellWithAmount>();
            cellScript.Setup(buyable);
            return cell;
        }
    }
}