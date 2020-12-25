using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Goat.UI
{
    public class AnimateOpenWindow : MonoBehaviour
    {
        [SerializeField] private Button windowOpener;
        [SerializeField] private Image windowOpenerImage;
        [SerializeField] private RectTransform toOpenWindow;
        [SerializeField] private RectTransform toOpenWindowBorderControl;
        [SerializeField] private AnimateWindowElements animateWindowElements;
        [SerializeField] private ClickedButtonAnimator buttonAnimator;
        [Title("Animation Settings")]
        [SerializeField, FoldoutGroup("Button scale punch")] private Vector3 scaleIncrease;
        [SerializeField, FoldoutGroup("Button scale punch"), Range(0f, 0.3f)] private float punchScaleDuration;
        [SerializeField, FoldoutGroup("Button scale punch"), Range(10, 30)] private int vibrato;
        [SerializeField, FoldoutGroup("Button scale punch"), Range(0f, 1f)] private float elasticity;

        private Sequence onButtonClick;

        private void Awake()
        {
            windowOpener.onClick.AddListener(OpenWindow);
        }

        private void OpenWindow()
        {
            if (onButtonClick.NotNull())
                onButtonClick.Complete();

            onButtonClick = DOTween.Sequence();
            onButtonClick.Append(windowOpener.transform.DOPunchScale(scaleIncrease, punchScaleDuration, vibrato, elasticity));

            buttonAnimator.Play((RectTransform)windowOpener.transform, windowOpenerImage.sprite, toOpenWindow, toOpenWindowBorderControl, animateWindowElements.Play, animateWindowElements.Close);
        }
    }
}