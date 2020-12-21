using UnityEngine;
using UnityAtoms;

namespace Goat.Audio
{
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/AudioEventChannel", fileName = "AudioEventChannel")]
    public class AudioEventChannel : AtomEvent<Audio>
    {
    }
}