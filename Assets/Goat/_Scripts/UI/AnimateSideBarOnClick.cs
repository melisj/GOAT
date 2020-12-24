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
    [SerializeField] private Sprite empty;
    [SerializeField] private Sprite emptySelected;
    [SerializeField] private GameObject headerObj;
    [SerializeField] private Image buildButtonIcon;
    [SerializeField] private RectTransform headerRect;

    [Title("Selected")]
    [SerializeField] private GameObject selectedObj;
    [SerializeField] private RectTransform selectedRect;

    [Title("Bar buttons")]
    [SerializeField] private RectTransform[] buttonRects;
    [SerializeField] private RectTransform[] headerRects;

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
        mainButton.onClick.AddListener(OpenSideBar);
    }

    private void OpenSideBar()
    {
        SubcribedEvent.Raise();
        isOpened = true;

        if (openSideBar.NotNull())
            openSideBar.Complete();
        openSideBar = DOTween.Sequence();
        openSideBar.AppendCallback(() => buildButtonIcon.sprite = emptySelected);
        openSideBar.AppendCallback(() => selectedObj.SetActive(true));
        openSideBar.Append(selectedRect.DOScaleY(1, 1));
        openSideBar.Join(headerRect.DOScaleY(1, 1));
        for (int i = 0; i < buttonRects.Length; i++)
        {
            openSideBar.Append(buttonRects[i].DOScale(Vector3.one, 1));
            openSideBar.Append(headerRects[i].DOScaleY(1, 1));
        }
    }

    private void CloseSideBar()
    {
        isOpened = false;

        if (closeSideBar.NotNull())
            closeSideBar.Complete();
        closeSideBar = DOTween.Sequence();
        closeSideBar.AppendCallback(() => buildButtonIcon.sprite = empty);
        closeSideBar.AppendCallback(() => selectedObj.SetActive(false));
        closeSideBar.Append(selectedRect.DOScaleY(0, 1));
        closeSideBar.Join(headerRect.DOScaleY(0, 1));
        for (int i = 0; i < buttonRects.Length; i++)
        {
            closeSideBar.Append(buttonRects[i].DOScale(Vector3.zero, 1));
            closeSideBar.Append(headerRects[i].DOScaleY(0, 1));
        }
    }
}