using UnityEngine;
using UnityEngine.UI;

namespace Goat.UI
{
    public class ProductsWindow : BaseUIWindow
    {
        protected override void SetupCell(Buyable buyable, Transform grid, GridLayoutGroup currentLayoutGroup)
        {
            base.SetupCell(buyable, grid, currentLayoutGroup);
            uiCell.OnClick((() => OnCellClick()));
        }

        private void OnCellClick()
        {
        }
    }
}