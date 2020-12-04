using Goat;
using Goat.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FadeGridOnChangeMode : EventListenerInputMode
{
    [SerializeField] private Renderer rend;
    [SerializeField] private float fadeDuration;
    private Sequence fadeSequence;

    private void Awake()
    {
        fadeSequence = DOTween.Sequence();
    }

    public override void OnEventRaised(InputMode value)
    {
        FadeGrid(value);
    }

    private void FadeGrid(InputMode inputMode)
    {
        if (inputMode == InputMode.Destroy | inputMode == InputMode.Edit)
        {
            if (fadeSequence.IsPlaying())
                fadeSequence.Complete();
            fadeSequence.Append(rend.material.DOFade(1, fadeDuration));
        }
        else
        {
            if (fadeSequence.IsPlaying())
                fadeSequence.Complete();
            fadeSequence.Append(rend.material.DOFade(0, fadeDuration));
        }
    }
}