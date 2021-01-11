using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class PlayAudioOnClick : AudioCue
{
    [SerializeField] private Button button;

    [Button]
    private void SetupButtonRef() { button = GetComponent<Button>(); }

    private void Awake()
    {
        button.onClick.AddListener(() => PlayAudioCue());
    }
}