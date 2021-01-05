using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Goat.AI;

public class TakeItemAudio : AudioCue
{
    [SerializeField] private NPC npc;

    private void OnEnable()
    {
        npc.takeItem.eventHandler += TakeItem_eventHandler;
    }

    private void OnDisable()
    {
        npc.takeItem.eventHandler -= TakeItem_eventHandler;
    }

    private void TakeItem_eventHandler(object sender, EventArgs e)
    {
        PlayAudioCue();
    }
}
