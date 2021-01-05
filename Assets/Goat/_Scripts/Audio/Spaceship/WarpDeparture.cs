using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpDeparture : AudioCue
{
    private void OnDisable()
    {
        PlayAudioCue();
    }
}
