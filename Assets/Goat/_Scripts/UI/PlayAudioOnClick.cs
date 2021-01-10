using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayAudioOnClick : AudioCue
{
    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(PlayAudioCue);
    }
}