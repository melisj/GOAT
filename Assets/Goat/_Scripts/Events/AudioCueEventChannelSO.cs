﻿using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// Event on which <c>AudioCue</c> components send a message to play SFX and music. <c>AudioManager</c> listens on these events, and actually plays the sound.
/// </summary>
[CreateAssetMenu(menuName = "Events/AudioCue Event Channel")]
public class AudioCueEventChannelSO : ScriptableObject
{
    public UnityAction<AudioCue, Vector3, GameObject> OnAudioCueRequested;
    public UnityAction<GameObject> OnAudioCueStopRequested;

    public void RaiseStopEvent(GameObject audioCue)
    {
        if (OnAudioCueStopRequested != null)
        {
            OnAudioCueStopRequested.Invoke(audioCue);
        }
        else
        {
            Debug.LogWarning("An AudioCue was requested, but nobody picked it up. " +
                "Check why there is no AudioManager already loaded, " +
                "and make sure it's listening on this AudioCue Event channel.");
        }
    }

    public void RaiseEvent(AudioCue audioCue, Vector3 positionInSpace, GameObject parent)
    {
        if (OnAudioCueRequested != null)
        {
            OnAudioCueRequested.Invoke(audioCue, positionInSpace, parent);
        }
        else
        {
            Debug.LogWarning("An AudioCue was requested, but nobody picked it up. " +
                "Check why there is no AudioManager already loaded, " +
                "and make sure it's listening on this AudioCue Event channel.");
        }
    }
}