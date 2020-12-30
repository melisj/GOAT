using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Sirenix.OdinInspector;
using UnityAtoms.BaseAtoms;

public class AnimateWindowElements : MonoBehaviour
{
    [SerializeField] private VoidEvent onCloseWindow;
    [SerializeField] private RectTransform tabButtonParent;
    [SerializeField] private WindowElement[] otherElements;
    [Title("Animation Settings")]
    [SerializeField, Range(2, 5)] private int closingDurationMultiplier;
    [SerializeField] private float tabButtonScaleDuration;
    [SerializeField] private float otherElementsScaleDuration;

    private Sequence animateElements;
    private Button firsTabButton;

    public void Play()
    {
        if (animateElements.NotNull())
            animateElements.Complete();
        if (!firsTabButton)
            firsTabButton = tabButtonParent.transform.GetChild(0).GetComponent<Button>();

        animateElements = DOTween.Sequence();

        animateElements.AppendCallback(() => firsTabButton.onClick.Invoke());

        for (int i = 0; i < tabButtonParent.transform.childCount; i++)
        {
            Transform child = tabButtonParent.transform.GetChild(i);
            animateElements.Append(child.DOScale(Vector3.one, tabButtonScaleDuration));
        }

        for (int i = 0; i < otherElements.Length; i++)
        {
            animateElements.Append(otherElements[i].RectTransform.DOScale(Vector3.one, otherElementsScaleDuration));
        }
    }

    public void Close()
    {
        onCloseWindow.Raise();

        for (int i = 0; i < tabButtonParent.transform.childCount; i++)
        {
            Transform child = tabButtonParent.transform.GetChild(i);
            animateElements.Append(child.DOScale(Vector3.zero, tabButtonScaleDuration / closingDurationMultiplier));
        }

        for (int i = 0; i < otherElements.Length; i++)
        {
            WindowElement element = otherElements[i];
            animateElements.Append(element.RectTransform.DOScale(element.DownScale, otherElementsScaleDuration / closingDurationMultiplier));
        }
    }
}

[System.Serializable]
public class WindowElement
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Vector3 downScale;

    public RectTransform RectTransform => rectTransform;
    public Vector3 DownScale => downScale;
}