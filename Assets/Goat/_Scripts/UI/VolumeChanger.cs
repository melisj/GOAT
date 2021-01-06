using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeChanger : MonoBehaviour
{
    [SerializeField] private AudioMixer mixerVolumeToChange;
    [SerializeField] private Slider volumeSlider;

    private void Awake()
    {
        volumeSlider.onValueChanged.AddListener((float s) => ChangeVolume());
    }

    private void ChangeVolume()
    {
        float newVolume = -80 + (80 * (volumeSlider.value));
        mixerVolumeToChange.SetFloat("MasterVolume", newVolume);
    }
}