using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Goat.Pooling;

[RequireComponent(typeof(AudioSource))]
public class SoundEmitter : MonoBehaviour, IPoolObject
{
    private AudioSource _audioSource;

    public int PoolKey { get; set; }
    public ObjectInstance ObjInstance { get; set; }
    public AudioSource AudioSource => _audioSource;

    public event UnityAction<SoundEmitter> OnSoundFinishedPlaying;

    public event UnityAction<SoundEmitter> OnSoundStopped;

    private void Awake()
    {
        Setup();
    }

    private void Setup()
    {
        if (!_audioSource)
            _audioSource = this.GetComponent<AudioSource>();
        if (_audioSource)
            _audioSource.playOnAwake = false;
    }

    public void OnGetObject(ObjectInstance objectInstance, int poolKey)
    {
        ObjInstance = objectInstance;
        PoolKey = poolKey;
    }

    public void OnReturnObject()
    {
        gameObject.name = $"Stopped: {_audioSource.clip.name}";

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Instructs the AudioSource to play a single clip, with optional looping, in a position in 3D space.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="settings"></param>
    /// <param name="hasToLoop"></param>
    /// <param name="position"></param>
    public void PlayAudioClip(AudioClip clip, AudioConfigurationSO settings, bool hasToLoop, Vector3 position = default)
    {
        settings.ApplyTo(_audioSource);
        _audioSource.clip = clip;
        _audioSource.transform.position = position;
        _audioSource.loop = hasToLoop;
        _audioSource.Play();

        gameObject.name = $"Playing: {clip.name}";
        if (!hasToLoop)
        {
            StartCoroutine(FinishedPlaying(clip.length));
        }
    }

    /// <summary>
    /// Used when the game is unpaused, to pick up SFX from where they left.
    /// </summary>
    public void Resume()
    {
        _audioSource.Play();
    }

    /// <summary>
    /// Used when the game is paused.
    /// </summary>
    public void Pause()
    {
        _audioSource.Pause();
    }

    /// <summary>
    /// Used when the SFX finished playing. Called by the <c>AudioManager</c>.
    /// </summary>
    public void Stop() // Redundant?
    {
        _audioSource.Stop();
    }

    public bool IsInUse()
    {
        return _audioSource.isPlaying;
    }

    public bool IsLooping()
    {
        return _audioSource.loop;
    }

    private IEnumerator FinishedPlaying(float clipLength)
    {
        yield return new WaitForSeconds(clipLength);

        OnSoundFinishedPlaying.Invoke(this); // The AudioManager will pick this up
    }
}