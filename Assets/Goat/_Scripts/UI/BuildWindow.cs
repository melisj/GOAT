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
        [SerializeField] private PlaceableEvent onPlaceableChosen;
        [SerializeField] private Sprite selectedBorder;
        [SerializeField] private Sprite emptyBorder;
        [SerializeField] private InputModeVariable inputMode;
        private Sequence clickSequence;
        private CellWithPrice previousCell;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void ShowUI()
        {
            base.ShowUI();
            inputMode.InputMode = InputMode.Select;
        }

        private void ChangeBorderImageBack()
        {
            if (previousCell)
            {
                previousCell.BorderImage.sprite = emptyBorder;
            }
        }

        private void OnCellClick(Buyable buyable, UICell uiCell)
        {
            onPlaceableChosen.Raise((Placeable)buyable);
            // ChangeBorderImageBack();
            Debug.Log(uiCell);
            ChangeBorderImage(uiCell);
        }

        private void ChangeBorderImage(UICell cell)
        {
            //if (uiCell is CellWithPrice cellWithPrice)
            //{
            //  previousCell = cellWithPrice;
            if (clickSequence.NotNull())
                clickSequence.Complete();

            clickSequence = DOTween.Sequence();
            clickSequence.Append(cell.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 1), 0.2f, 15, 0.5f));
            //   clickSequence.PrependInterval(0.1f).OnComplete(() => { cellWithPrice.BorderImage.sprite = selectedBorder; });
            //}
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