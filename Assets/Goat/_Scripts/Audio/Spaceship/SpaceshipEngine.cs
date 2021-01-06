using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipEngine : AudioCue
{
    private void OnEnable()
    {
        PlayAudioCue();
    }

    private void OnDisable()
    {
        StopAudioCue();
    }
}
