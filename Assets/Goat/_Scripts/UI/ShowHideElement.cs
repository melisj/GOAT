using UnityEngine;
using Sirenix.OdinInspector;

public enum UIHide
{
    transform,
    canvas
}

[System.Serializable]
public class ShowHideElement
{
    [SerializeField, ShowIf("uiHideType", UIHide.transform)] private RectTransform transform;
    [SerializeField, ShowIf("uiHideType", UIHide.canvas)] private Canvas canvas;
    [SerializeField] private bool show;
    [SerializeField, EnumToggleButtons()] private UIHide uiHideType;
    public bool Show => show;
    public RectTransform Transform => transform;
    public Canvas Canvas => canvas;

    public UIHide UiHideType => uiHideType;
}