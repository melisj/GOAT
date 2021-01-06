using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class WalkingAudio : AudioCue
{
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private float delay = 0.25f;
    private bool walking;
    private Sequence walkSequence;

    private void Awake()
    {
        walkSequence = DOTween.Sequence();
        walkSequence.SetLoops(-1);
        walkSequence.AppendInterval(delay);
        walkSequence.AppendCallback(PlayAudio);
    }

    private void PlayAudio()
    {
        if (!navAgent.enabled) return;

        if (navAgent.velocity.sqrMagnitude > 0.01f)
            PlayAudioCue();
        else
            StopAudioCue();
    }

    //private void Update()
    //{
    //    if (navAgent.enabled)
    //    {
    //        if(navAgent.velocity.sqrMagnitude > 0.01f && !walking)
    //        {
    //            walking = true;
    //            PlayAudioCue();
    //        }
    //        else if(navAgent.velocity.sqrMagnitude <= 0.01f && walking)
    //        {
    //            walking = false;
    //            StopAudioCue();
    //        }
    //    }
    //}
}