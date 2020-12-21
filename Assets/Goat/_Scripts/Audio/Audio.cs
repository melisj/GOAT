using UnityEngine;
using Goat.Events;

namespace Goat.Audio
{
    public class Audio : IPairable<AudioCue, AudioConfiguration>
    {
        [SerializeField] private AudioCue cue;
        [SerializeField] private AudioConfiguration audioConfiguration;

        public AudioCue Item1 { get => cue; set => cue = value; }
        public AudioConfiguration Item2 { get => audioConfiguration; set => audioConfiguration = value; }

        public void Deconstruct(out AudioCue cue, out AudioConfiguration audioConfiguration)
        {
            cue = Item1;
            audioConfiguration = Item2;
        }
    }
}