using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Goat.Pooling;
using DG.Tweening;
using UnityAtoms;
using UnityAtoms.BaseAtoms;

public class AudioManager : MonoBehaviour, IAtomListener<int>
{
    [Header("SoundEmitters pool")]
    [SerializeField] private SoundEmitterFactorySO _factory = default;
    [Header("Listening on channels")]
    [Tooltip("The SoundManager listens to this event, fired by objects in any scene, to play SFXs")]
    [SerializeField] private AudioCueEventChannelSO _SFXEventChannel = default;
    [Tooltip("The SoundManager listens to this event, fired by objects in any scene, to play Music")]
    [SerializeField] private AudioCueEventChannelSO _musicEventChannel = default;
    private Dictionary<GameObject, List<SoundEmitter>> audioCuesCreated;
    [Header("Audio control")]
    [SerializeField] private IntEvent onTimeSpeedChanged;
    [SerializeField] private AudioMixer audioMixer = default;
    [Range(0f, 1f)]
    [SerializeField] private float _masterVolume = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float _musicVolume = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float _sfxVolume = 1f;
    private float currentPitchSFX;

    private void Awake()
    {
        //TODO: Get the initial volume levels from the settings
        audioCuesCreated = new Dictionary<GameObject, List<SoundEmitter>>();
        _SFXEventChannel.OnAudioCueStopRequested += StopAudioCue;
        _musicEventChannel.OnAudioCueStopRequested += StopAudioCue;
        audioMixer.GetFloat("SFXPitch", out currentPitchSFX);
        onTimeSpeedChanged.RegisterSafe(this);
        _SFXEventChannel.OnAudioCueRequested += PlayAudioCue;
        _musicEventChannel.OnAudioCueRequested += PlayMusicCue; //TODO: Treat music requests differently?
    }

    /// <summary>
    /// This is only used in the Editor, to debug volumes.
    /// It is called when any of the variables is changed, and will directly change the value of the volumes on the AudioMixer.
    /// </summary>
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            SetGroupVolume("MasterVolume", _masterVolume);
            SetGroupVolume("MusicVolume", _musicVolume);
            SetGroupVolume("SFXVolume", _sfxVolume);
        }
    }

    public void SetGroupVolume(string parameterName, float normalizedVolume)
    {
        bool volumeSet = audioMixer.SetFloat(parameterName, NormalizedToMixerValue(normalizedVolume));
        if (!volumeSet)
            Debug.LogError("The AudioMixer parameter was not found");
    }

    public float GetGroupVolume(string parameterName)
    {
        if (audioMixer.GetFloat(parameterName, out float rawVolume))
        {
            return MixerValueToNormalized(rawVolume);
        }
        else
        {
            Debug.LogError("The AudioMixer parameter was not found");
            return 0f;
        }
    }

    // Both MixerValueNormalized and NormalizedToMixerValue functions are used for easier transformations
    /// when using UI sliders normalized format
    private float MixerValueToNormalized(float mixerValue)
    {
        // We're assuming the range [-80dB to 0dB] becomes [0 to 1]
        return 1f + (mixerValue / 80f);
    }

    private float NormalizedToMixerValue(float normalizedValue)
    {
        // We're assuming the range [0 to 1] becomes [-80dB to 0dB]
        // This doesn't allow values over 0dB
        return (normalizedValue - 1f) * 80f;
    }

    /// <summary>
    /// Plays an AudioCue by requesting the appropriate number of SoundEmitters from the pool.
    /// </summary>
    public void PlayAudioCue(AudioCue cue, Vector3 position = default, Transform parent = null)
    {
        AudioCueSO audioCue = cue.GetAudioCue;
        AudioConfigurationSO settings = cue.AudioConfiguration;

        AudioClip[] clipsToPlay = audioCue.GetClips();
        int nOfClips = clipsToPlay.Length;
        List<SoundEmitter> soundEmitters = new List<SoundEmitter>();
        for (int i = 0; i < nOfClips; i++)
        {
            SoundEmitter soundEmitter = _factory.Create(position, parent);
            if (soundEmitter != null)
            {
                soundEmitters.Add(soundEmitter);
                soundEmitter.PlayAudioClip(clipsToPlay[i], settings, audioCue.looping, position);
                if (!audioCue.looping)
                    soundEmitter.OnSoundFinishedPlaying += OnSoundEmitterFinishedPlaying;
            }
        }

        if (!audioCuesCreated.ContainsKey(cue.gameObject))
            audioCuesCreated.Add(cue.gameObject, soundEmitters);

        //TODO: Save the SoundEmitters that were activated, to be able to stop them if needed
    }

    public void PlayMusicCue(AudioCue cue, Vector3 pos = default, Transform parent = null)
    {
        if (audioCuesCreated.ContainsKey(cue.gameObject))
            StopAudioCue(cue.gameObject);
        PlayAudioCue(cue, pos, parent);
    }

    public void StopAudioCue(GameObject audioCue)
    {
        if (!audioCuesCreated.ContainsKey(audioCue)) return;
        audioCuesCreated.TryGetValue(audioCue, out List<SoundEmitter> soundEmitters);

        for (int i = 0; i < soundEmitters.Count; i++)
        {
            soundEmitters[i].Stop();
            PoolManager.Instance.ReturnToPool(soundEmitters[i].gameObject);
        }

        audioCuesCreated.Remove(audioCue);
    }

    private void OnSoundEmitterFinishedPlaying(SoundEmitter soundEmitter)
    {
        soundEmitter.OnSoundFinishedPlaying -= OnSoundEmitterFinishedPlaying;
        soundEmitter.Stop();
        PoolManager.Instance.ReturnToPool(soundEmitter.gameObject);
    }

    private void CrossFade(AudioClip currentClip, AudioClip nextClip)
    {
    }

    public void OnEventRaised(int timeSpeed)
    {
        audioMixer.SetFloat("SFXPitch", currentPitchSFX * Time.timeScale);
    }

    //TODO: Add methods to play and cross-fade music, or to play individual sounds?
}