using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class SetVisibilityUIElementsOnClick : MonoBehaviour
{
    [SerializeField] private Button activator;
    [SerializeField] private Button hider;
    [SerializeField] private RectTransform[] uiElements;
    [Title("Animation Settings")]
    [SerializeField] private float scalingDuration;
    [SerializeField, Range(2, 5)] private int closingMultiplier;
    private Sequence scalingSequence;

    private void Awake()
    {
    }

    private void SetVisibility(bool setVisible)
    {
        if (scalingSequence.NotNull())
            scalingSequence.Complete();
    }
}