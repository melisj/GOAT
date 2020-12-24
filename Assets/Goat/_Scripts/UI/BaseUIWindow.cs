﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Goat.Events;
using System;

namespace Goat.UI
{
    public class BaseUIWindow : MonoBehaviour
    {
        [SerializeField] protected GameObject cellPrefab;
        [SerializeField] protected UIGridCell[] gridCells;
        protected GameObject cell;
        protected UICell uiCell;

        protected virtual void Awake()
        {
            SetupGrid();
        }

        private void SetupGrid()
        {
            for (int i = 0; i < gridCells.Length; i++)
            {
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