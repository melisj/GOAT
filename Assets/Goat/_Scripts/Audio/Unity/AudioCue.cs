using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Simple implementation of a MonoBehaviour that is able to request a sound being played by the <c>AudioManager</c>.
/// It fires an event on an <c>AudioCueEventSO</c> which acts as a channel, that the <c>AudioManager</c> will pick up and play.
/// </summary>
public class AudioCue : MonoBehaviour
{
    [Header("Sound definition")]
    [SerializeField, InlineEditor] private AudioCueSO _audioCue = default;
    [SerializeField] private bool _playOnStart = false;
    [SerializeField] protected Transform parent;
    [Header("Configuration")]
    [SerializeField] private AudioCueEventChannelSO _audioCueEventChannel = default;
    [SerializeField, InlineEditor] private AudioConfigurationSO _audioConfiguration = default;

    public AudioCueSO GetAudioCue { get => _audioCue; set => _audioCue = value; }
    public AudioConfigurationSO AudioConfiguration { get => _audioConfiguration; set => _audioConfiguration = value; }

    private void Start()
    {
        if (_playOnStart)
            PlayAudioCue();
    }

    public void PlayAudioCue()
    {
        if (!parent)
        {
            parent = transform;
        }
        _audioCueEventChannel.RaiseEvent(this, parent.position, parent);
    }

    public void StopAudioCue()
    {
        _audioCueEventChannel.RaiseStopEvent(gameObject);
    }
}