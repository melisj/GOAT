﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpDeparture : AudioCue
{
    public void PlayAudio(Transform parent)
    {
        this.parent = parent;
        PlayAudioCue();
    }
}