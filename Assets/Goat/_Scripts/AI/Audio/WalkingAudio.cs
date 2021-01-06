using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkingAudio : AudioCue
{
    [SerializeField] private NavMeshAgent navAgent;
    private bool walking;

    private void Update()
    {
        if (navAgent.enabled)
        {
            if(navAgent.velocity.sqrMagnitude > 0.01f && !walking)
            {
                walking = true;
                PlayAudioCue();
            }
            else if(navAgent.velocity.sqrMagnitude <= 0.01f && walking)
            {
                walking = false;
                StopAudioCue();
            }
        }
    }

}
