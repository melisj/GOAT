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
            mixerVolumeToChange.SetFloat(volumeToChange, Mathf.Log10(volumeSlider.value) * 20 );
        else
            audioManager.SetGroupVolume(volumeToChange, Mathf.Log10(volumeSlider.value) * 20 );
    }
}