using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicPlayed", menuName = "ScriptableObjects/GlobalVariables/MusicPlayed")]
public class MusicPlayed : ScriptableObject
{
    [SerializeField] private List<SoundEmitter> musicEmitters;

    public List<SoundEmitter> MusicEmitters
    {
        get => musicEmitters;
        set => musicEmitters = value;
    }
}