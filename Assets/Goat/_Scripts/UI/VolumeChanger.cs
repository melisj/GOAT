using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeChanger : MonoBehaviour
{
    [SerializeField] private AudioMixer mixerVolumeToChange;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private string volumeToChange;
    [SerializeField] private Slider volumeSlider;

    private void Awake()
    {
        volumeSlider.onValueChanged.AddListener((float s) => ChangeVolume());
    }

    private void ChangeVolume()
    {
        if (!audioManager)
            mixerVolumeToChange.SetFloat(volumeToChange, (volumeSlider.value - 1f) * 80f);
        else
            audioManager.SetGroupVolume(volumeToChange, volumeSlider.value);
    }
}