﻿using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BorderContentFitter : MonoBehaviour
{
    [SerializeField] private RectTransform borderLeft;
    [SerializeField] private RectTransform borderMiddle;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float margin;

    [Button]
    protected void ChangeIconWidth()
    {
        //InitialSize
        float initialSize = borderLeft.sizeDelta.x * 2;
        float iconWidth = ((text.fontSize) + ((text.fontSize + margin) * (text.text.Length)));
        iconWidth -= initialSize;
        iconWidth = Mathf.Max(iconWidth, 1);
        borderMiddle.sizeDelta = new Vector2(iconWidth, borderMiddle.sizeDelta.y);
    }
}