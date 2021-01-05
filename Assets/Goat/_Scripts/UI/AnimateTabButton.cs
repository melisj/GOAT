using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using Goat.Events;
using UnityAtoms;
using System;

public class AnimateTabButton : EventListenerVoid
{
    [SerializeField, Sirenix.OdinInspector.ReadOnly] protected Transform[] tabPositions;
    [SerializeField] private GameObject viewPort;
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private ScrollRect scrollingRect;
    [SerializeField] private RectTransform selectionBlock;
    [SerializeField] private Sprite emptySelection;
    [SerializeField] private Sprite empty;

    [Title("Animation Settings")]
    [SerializeField, Range(2, 5)] private int closingDurationMultiplier;
    [SerializeField] private float selectionBlockScaleDuration;
    [SerializeField] private float selectionBlockMoveDuration;
    [SerializeField] private float contentScaleDuration;

    public event EventHandler<int> OnTabSwitch;

    private int currentTab;
    private GameObject currentGrid;
    private Image currentTabBorder;
    private Sequence tabSequence;

    private void Awake()
    {
        SetupTabPositions();
        Setup();
    }

    /// <summary>
    /// Fills the tabPositions array
    /// And GetsAllComponents needed
    /// </summary>
    private void SetupTabPositions()
    {
        tabPositions = new Transform[gameObject.transform.childCount];
        for (int i = 0; i < tabPositions.Length; i++)
        {
            RectTransform tabChild = gameObject.transform.GetChild(i).GetComponent<RectTransform>();
            tabPositions[i] = tabChild.transform;
        }
    }

    private void Setup()
    {
        for (int i = 0; i < tabPositions.Length; i++)
        {
            RectTransform tabChild = gameObject.transform.GetChild(i).GetComponent<RectTransform>();
            RectTransform gridChild = viewPort.transform.GetChild(i).GetComponent<RectTransform>();
            GridLayoutGroup layoutGroup = viewPort.transform.GetChild(i).GetComponent<GridLayoutGroup>();

            SetupPairs(i, tabChild, gridChild, gridChild, layoutGroup);
        }

        currentTabBorder = gameObject.transform.GetChild(0).GetComponent<Image>();
        currentGrid = viewPort.transform.GetChild(0).gameObject;
    }

    /// <summary>
    /// Setups the OnClick event to push needed data to selectTab
    /// </summary>
    private void SetupPairs(int index, Transform tab, Transform grid, RectTransform content, GridLayoutGroup layoutGroup)
    {
        Button tabButton = tab.GetComponent<Button>();
        Image tabBorder = tab.GetComponent<Image>();
        tabButton.onClick.AddListener(() => SelectTab(index, tabBorder, grid.gameObject, content, layoutGroup));
    }

    /// <summary>
    /// Deactivate current grid and activate new selected grid
    /// Lerp the position between old and new for animation
    /// </summary>
    /// <param name="tabIndex"></param>
    /// <param name="grid"></param>
    private void SelectTab(int tabIndex, Image tabBorder, GameObject grid, RectTransform content, GridLayoutGroup layoutGroup)
    {
        //float oldPos = tabPositions[currentTab];
        if (tabIndex >= tabPositions.Length)
        {
            tabIndex = 0;
        }

        HideCurrentGrid();

        grid.SetActive(true);
        currentGrid = grid;
        currentTabBorder = tabBorder;
        currentTab = tabIndex;
        scrollingRect.content = content;

        header.text = currentTabBorder.gameObject.name;
        //tabSequence.Append(selectionBlock.DOScaleX(0, selectionBlockScaleDuration / closingDurationMultiplier));
        tabSequence.Append(selectionBlock.transform.DOMoveY(tabPositions[currentTab].position.y, selectionBlockMoveDuration));
        tabSequence.Append(selectionBlock.DOScaleX(1, selectionBlockScaleDuration).OnComplete(() => currentTabBorder.sprite = emptySelection));

        for (int i = 0; i < content.childCount; i++)
        {
            tabSequence.Append(content.GetChild(i).DOScale(Vector3.one, contentScaleDuration));
        }
        OnTabSwitch?.Invoke(this, tabIndex);
    }

    private void HideCurrentGrid()
    {
        if (!currentGrid) return;

        currentTabBorder.sprite = empty;

        if (tabSequence.NotNull())
            tabSequence.Complete();

        tabSequence = DOTween.Sequence();
        tabSequence.Append(selectionBlock.DOScaleX(0, selectionBlockScaleDuration / closingDurationMultiplier));

        for (int i = 0; i < currentGrid.transform.childCount; i++)
        {
            tabSequence.Append(currentGrid.transform.GetChild(i).DOScale(Vector3.zero, contentScaleDuration / closingDurationMultiplier));
        }
    }

    public override void OnEventRaised(UnityAtoms.Void value)
    {
        HideCurrentGrid();
    }
}