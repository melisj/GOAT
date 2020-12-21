using UnityEngine;
using UnityEngine.UI;

public class AudioCueOnMenuClick : AudioCue
{
    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(PlayAudioCue);
    }
}