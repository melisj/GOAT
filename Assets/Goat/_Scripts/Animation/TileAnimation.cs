using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class TileAnimation : MonoBehaviour
{
    [SerializeField] private float createDuration = 0.2f;
    [SerializeField, Range(2, 5)] private int destroyDurationMultiplier;
    [SerializeField] private Vector3 scaleDown;
    private Sequence createSequence, destroySequence;

    public void Create(Action onComplete = null)
    {
        if (destroySequence.NotNull())
            destroySequence.Complete();
        if (createSequence.NotNull())
            createSequence.Complete();

        gameObject.SetActive(true);
        createSequence = DOTween.Sequence();
        if (onComplete != null)
            createSequence.OnComplete(() => onComplete());
        createSequence.Append(transform.DOScale(Vector3.one, createDuration));
    }

    public void Destroy(Action onComplete = null)
    {
        if (createSequence.NotNull())
            createSequence.Complete();
        if (destroySequence.NotNull())
            destroySequence.Complete();

        destroySequence = DOTween.Sequence();
        if (onComplete != null)
            destroySequence.OnComplete(() => onComplete());
        destroySequence.Append(transform.DOScale(scaleDown, createDuration / destroyDurationMultiplier));
    }

    public void Prepare()
    {
        transform.localScale = scaleDown;
    }
}