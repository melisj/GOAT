using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpArrival : AudioCue
{
    public void PlayAudio(GameObject parent)
    {
        this.parent = parent;
        PlayAudioCue();
    }
}
