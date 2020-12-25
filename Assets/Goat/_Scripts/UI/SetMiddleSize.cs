using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMiddleSize : MonoBehaviour
{
    private const int SPRITE_SIZE = 48;

    [SerializeField] private RectTransform windowTransform;
    [SerializeField] private UIMiddleChange[] middleUIs;

    private void OnValidate()
    {
        ChangeSize();
    }

    [Button]
    public void ChangeSize()
    {
        Vector2 newSize = Vector2.zero;
        if (windowTransform && middleUIs != null)
        {
            for (int i = 0; i < middleUIs.Length; i++)
            {
                UIMiddleChange midUI = middleUIs[i];
                newSize.x = midUI.MiddleAxis == MiddleAxis.x || midUI.MiddleAxis == MiddleAxis.xy ? windowTransform.sizeDelta.x - (SPRITE_SIZE * 2) : SPRITE_SIZE;
                newSize.y = midUI.MiddleAxis == MiddleAxis.y || midUI.MiddleAxis == MiddleAxis.xy ? windowTransform.sizeDelta.y - (SPRITE_SIZE * 2) : SPRITE_SIZE;
                middleUIs[i].Transform.sizeDelta = newSize;
            }
        }
    }
}

[System.Serializable]
public class UIMiddleChange
{
    [SerializeField] private MiddleAxis middleAxis;
    [SerializeField] private RectTransform transform;

    public MiddleAxis MiddleAxis { get => middleAxis; set => middleAxis = value; }
    public RectTransform Transform { get => transform; set => transform = value; }
}

public enum MiddleAxis
{
    x,
    y,
    xy
}