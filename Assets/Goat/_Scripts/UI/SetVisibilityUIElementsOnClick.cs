using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using Goat.UI;

namespace Goat.UI
{
    public class SetVisibilityUIElementsOnClick : MonoBehaviour
    {
        [SerializeField] private Button activator;
        [SerializeField] private Button hider;
        [SerializeField] private WindowElement[] uiElements;
        [Title("Animation Settings")]
        [SerializeField] private float scalingDuration;
        [SerializeField, Range(2, 5)] private int closingMultiplier;
        private Sequence scalingSequence;

        private void Awake()
        {
            for (int i = 0; i < uiElements.Length; i++)
            {
                uiElements[i].RectTransform.localScale = uiElements[i].DownScale;
            }
            activator.onClick.AddListener(() => SetVisibility(true));
            hider.onClick.AddListener(() => SetVisibility(false));
        }

        private void SetVisibility(bool setVisibility)
        {
            if (scalingSequence.NotNull())
                scalingSequence.Complete();

            for (int i = 0; i < uiElements.Length; i++)
            {
                WindowElement element = uiElements[i];

                if (setVisibility)
                    scalingSequence.Append(element.RectTransform.DOScale(Vector3.one, scalingDuration));
                else
                    scalingSequence.Append(element.RectTransform.DOScale(element.DownScale, scalingDuration / closingMultiplier));
            }
        }
    }
}