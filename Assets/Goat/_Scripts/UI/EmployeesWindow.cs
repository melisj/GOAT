using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityAtoms;
using System;

namespace Goat.UI
{
    public class EmployeesWindow : SupplyWindow
    {
        [SerializeField] private TextMeshProUGUI hireButtonText;
        private UICell selectedCell;
        private float multiplier;

        protected override void Buy()
        {
            if (!AnimateBuyButton(currentAmount > 0 && !selectedBuyable.CanBuy(currentAmount))) return;
            selectedBuyable.Buy(currentAmount);
        }

        protected override void TabSwitcher_OnTabSwitch(object sender, int e)
        {
            GameObject firstCell = gridCells[e].Transform.GetChild(0).gameObject;
            UICell cellScript = firstCell.GetComponent<UICell>();
            if (cellScript)
            {
                cellScript.InvokeOnClick();
            }
        }

        protected override void SetupCell(Buyable buyable, Transform grid, GridLayoutGroup currentLayoutGroup)
        {
            base.SetupCell(buyable, grid, currentLayoutGroup);
            UICell cellScript = cell.GetComponent<UICell>();
            if (cellScript is CellWithAmount)
                cellScript.OnClick(() => OnFireCellClick(buyable));
            else
                cellScript.OnClick(() => OnHireCellClick(buyable));
        }

        private void OnHireCellClick(Buyable buyable)
        {
            base.OnCellClick(buyable);
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(Buy);
            hireButtonText.text = "Hire";
            multiplier = 1;
        }

        private void OnFireCellClick(Buyable buyable)
        {
            base.OnCellClick(buyable);
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(Fire);
            hireButtonText.text = "Fire";
            multiplier = 0.75f;
        }

        protected override void SetTotalPrice()
        {
            totalPrice.text = (currentAmount * selectedBuyable.Price * multiplier).ToString("N0");
        }

        private void Fire()
        {
            if (!AnimateBuyButton(currentAmount > 0 && selectedBuyable.Amount - currentAmount >= 0)) return;
            selectedBuyable.Sell(currentAmount);
        }
    }
}