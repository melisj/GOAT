using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Goat.Events;
using System;
using Goat.Grid.UI;

namespace Goat.UI
{
    public class BaseUIWindow : BasicGridUIElement
    {
        [SerializeField] protected GameObject cellPrefab;
        [SerializeField] protected UIGridCell[] gridCells;
        protected GameObject cell;
        protected UICell uiCell;
        private GameObject defaultPrefab;

        protected virtual void Awake()
        {
            defaultPrefab = cellPrefab;
            SetupGrid();
        }

        private void SetupGrid()
        {
            for (int i = 0; i < gridCells.Length; i++)
            {
                if (gridCells[i].UseDifferentPrefabs)
                    cellPrefab = gridCells[i].OtherPrefab;
                else
                    cellPrefab = defaultPrefab;

                CreateGridCells(gridCells[i].Transform, gridCells[i].ResourcePath);
            }
        }

        protected void CreateGridCells(RectTransform grid, string resourcesPath)
        {
            GridLayoutGroup gridGroup = grid.GetComponent<GridLayoutGroup>();
            Buyable[] buyableCells = Resources.LoadAll<Buyable>(resourcesPath);

            for (int i = 0; i < buyableCells.Length; i++)
            {
                Buyable buyableCell = buyableCells[i];
                SetupCell(buyableCell, grid, gridGroup);
            }
        }

        protected virtual void SetupCell(Buyable buyable, Transform grid, GridLayoutGroup currentLayoutGroup)
        {
            cell = Instantiate(cellPrefab, grid);
            uiCell = cell.GetComponent<UICell>();
            uiCell.Setup(buyable);
        }
    }
}