using UnityEngine;

[System.Serializable]
public class ShowHideElement
{
    [SerializeField] private RectTransform transform;
    [SerializeField] private bool show;

    public bool Show => show;
    public RectTransform Transform => transform;
}