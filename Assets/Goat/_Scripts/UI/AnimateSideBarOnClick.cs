using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Goat.Events;
using UnityAtoms;

public class AnimateSideBarOnClick : EventListenerVoid
{
    [Title("Bar")]
    [SerializeField] private Button mainButton;
    [Title("BuildButton")]
    [SerializeField] private SelectDeselectSprites selectionSprites;
    [SerializeField] private GameObject headerObj;
    [SerializeField] private Image buildButtonIcon;
    [SerializeField] private RectTransform headerRect;

    [Title("Selected")]
    [SerializeField] private GameObject selectedObj;
    [SerializeField] private RectTransform selectedRect;

    [TitleGroup("Bar buttons")]
    [SerializeField, HorizontalGroup("Bar buttons/Group", LabelWidth = 50)] private RectTransform[] buttonRects, headerRects;

    [Title("Animation Settings")]
    [SerializeField] private float barScaleDuration;
    [SerializeField] private float mainHeaderDuration;
    [SerializeField] private float buttonsAppearDuration;
    [SerializeField, Range(2, 5)] private int closingDurationMultiplier;
    [SerializeField, FoldoutGroup("Bar button scale punch")] private Vector3 scaleIncrease;
    [SerializeField, FoldoutGroup("Bar button scale punch"), Range(0f, 0.3f)] private float punchScaleDuration;
    [SerializeField, FoldoutGroup("Bar button scale punch"), Range(10, 30)] private int vibrato;
    [SerializeField, FoldoutGroup("Bar button scale punch"), Range(0f, 1f)] private float elasticity;
    private bool isOpened;
    private Sequence openSideBar, closeSideBar;

    /// <summary>
    /// When a new hotbar is open close yours
    /// </summary>
    /// <param name="value"></param>
    public override void OnEventRaised(Void value)
    {
        if (isOpened)
        {
            CloseSideBar();
        }
    }

    private void Awake()
    {
        ResetScale();
        isOpened = false;
        mainButton.onClick.AddListener(OpenSideBar);
    }

    private void OpenSideBar()
    {
        if (isOpened)
        {
            CloseSideBar();
            return;
        }

        //Let all other bars know you want to open
        SubcribedEvent.Raise();

        isOpened = true;
        if (closeSideBar.NotNull())
            closeSideBar.Complete();
        if (openSideBar.NotNull())
            openSideBar.Complete();
        openSideBar = DOTween.Sequence();
        openSideBar.AppendCallback(() => buildButtonIcon.sprite = selectionSprites.Selected);
        openSideBar.AppendCallback(() => selectedObj.SetActive(true));
        openSideBar.Append(buildButtonIcon.rectTransform.DOPunchScale(scaleIncrease, punchScaleDuration, vibrato, elasticity));

        openSideBar.Join(selectedRect.DOScaleX(1, barScaleDuration));
        openSideBar.Join(headerRect.DOScaleY(1, mainHeaderDuration));
        for (int i = 0; i < buttonRects.Length; i++)
        {
            openSideBar.Append(buttonRects[i].DOScale(Vector3.one, buttonsAppearDuration));
            openSideBar.Append(buttonRects[i].DOPunchScale(scaleIncrease, punchScaleDuration / closingDurationMultiplier, vibrato, elasticity));
            openSideBar.Append(headerRects[i].DOScaleY(1, buttonsAppearDuration));
        }
    }

    private void CloseSideBar()
    {
        isOpened = false;

        if (openSideBar.NotNull())
            openSideBar.Complete();

        if (closeSideBar.NotNull())
            closeSideBar.Complete();

        closeSideBar = DOTween.Sequence();
        closeSideBar.Join(buildButtonIcon.rectTransform.DOPunchScale(-scaleIncrease, punchScaleDuration, vibrato, elasticity));

        for (int i = buttonRects.Length - 1; i >= 0; i--)
        {
            closeSideBar.Join(buttonRects[i].DOScale(Vector3.zero, buttonsAppearDuration / closingDurationMultiplier));
            closeSideBar.Join(headerRects[i].DOScaleY(0, buttonsAppearDuration / closingDurationMultiplier));
        }

        closeSideBar.Join(headerRect.DOScaleY(0, mainHeaderDuration / closingDurationMultiplier));
        closeSideBar.Join(selectedRect.DOScaleX(0, barScaleDuration / closingDurationMultiplier));
        closeSideBar.AppendCallback(() => buildButtonIcon.sprite = selectionSprites.Deselected);
        closeSideBar.AppendCallback(() => selectedObj.SetActive(false));
    }

    private void ResetScale()
    {
        for (int i = buttonRects.Length - 1; i >= 0; i--)
        {
            buttonRects[i].localScale = Vector3.zero;
            headerRects[i].localScale = Vector3.right;
        }

        headerRect.localScale = Vector3.right;
        selectedRect.localScale = Vector3.up;
        buildButtonIcon.sprite = selectionSprites.Deselected;
        selectedObj.SetActive(false);
    }
}