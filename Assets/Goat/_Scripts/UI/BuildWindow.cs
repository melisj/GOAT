using Goat.Events;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityAtoms.BaseAtoms;
using UnityAtoms;
using System;

namespace Goat.UI
{
    public class BuildWindow : BaseUIWindow
    {
        private const string devPath = "ResourceTiles";

        [SerializeField] private PlaceableEvent onPlaceableChosen;
        [SerializeField] private InputModeVariable inputMode;
        private Sequence clickSequence;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void ShowUI()
        {
            base.ShowUI();
            inputMode.InputMode = InputMode.Select;
        }

        protected override void CreateGridCells(RectTransform grid, string resourcesPath)
        {
#if DEVELOPMENT_BUILD || !UNITY_EDITOR
            if(resourcesPath == devPath) return;
#endif
            base.CreateGridCells(grid, resourcesPath);
        }

        private void OnCellClick(Buyable buyable, UICell uiCell)
        {
            onPlaceableChosen.Raise((Placeable)buyable);
            ChangeBorderImage(uiCell);
        }

        private void ChangeBorderImage(UICell cell)
        {
            if (clickSequence.NotNull())
                clickSequence.Complete();

            clickSequence = DOTween.Sequence();
            clickSequence.Append(cell.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 1), 0.2f, 15, 0.5f));
        }

        protected override void SetupCell(Buyable buyable, Transform grid, GridLayoutGroup currentLayoutGroup)
        {
            base.SetupCell(buyable, grid, currentLayoutGroup);
            UICell newCell = uiCell;

            uiCell.OnClick(() =>
            {
                inputMode.InputMode = InputMode.Edit;
                OnCellClick(buyable, newCell);
            });
        }
    }
}