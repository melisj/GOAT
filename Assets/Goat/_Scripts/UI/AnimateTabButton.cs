using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;

public class AnimateTabButton : MonoBehaviour
{
    [SerializeField, ReadOnly] protected float[] tabPositions;
    [SerializeField] private GameObject viewPort;
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private VerticalLayoutGroup verticalGroup;
    [SerializeField] private ScrollRect scrollingRect;
    [SerializeField] private RectTransform selectionBlock;
    [SerializeField] private Sprite emptySelection;
    [SerializeField] private Sprite empty;

    private int currentTab;
    private GameObject currentGrid;
    private Image currentTabBorder;
    private Sequence tabSequence;

    private void Awake()
    {
        SetupTabPositions();
    }

    /// <summary>
    /// Fills the tabPositions array
    /// And GetsAllComponents needed
    /// </summary>
    private void SetupTabPositions()
    {
        tabPositions = new float[gameObject.transform.childCount];
        verticalGroup.enabled = true;
        Canvas.ForceUpdateCanvases();

        for (int i = 0; i < tabPositions.Length; i++)
        {
            RectTransform tabChild = gameObject.transform.GetChild(i).GetComponent<RectTransform>();
            verticalGroup.enabled = false;
            Canvas.ForceUpdateCanvases();
            tabPositions[i] = tabChild.anchoredPosition.y;
            RectTransform gridChild = viewPort.transform.GetChild(i).GetComponent<RectTransform>();
            GridLayoutGroup layoutGroup = viewPort.transform.GetChild(i).GetComponent<GridLayoutGroup>();

            SetupPairs(i, tabChild, gridChild, gridChild, layoutGroup);
        }
        verticalGroup.enabled = true;
        Canvas.ForceUpdateCanvases();
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
        float oldPos = tabPositions[currentTab];
        if (tabIndex >= tabPositions.Length)
        {
            tabIndex = 0;
        }

        currentTabBorder.sprite = empty;
        currentGrid.SetActive(false);

        grid.SetActive(true);
        currentGrid = grid;
        currentTabBorder = tabBorder;
        currentTab = tabIndex;
        scrollingRect.content = content;

        if (tabSequence.NotNull())
            tabSequence.Complete();

        tabSequence = DOTween.Sequence();
        tabSequence.Append(selectionBlock.DOScaleX(0, 0.1f));
        tabSequence.AppendCallback(() => currentTabBorder.sprite = emptySelection);
        tabSequence.Append(selectionBlock.DOAnchorPosY(tabPositions[currentTab], 0.1f));
        tabSequence.Join(selectionBlock.DOScaleX(1, 0.2f));
        header.text = currentTabBorder.gameObject.name;
    }
}