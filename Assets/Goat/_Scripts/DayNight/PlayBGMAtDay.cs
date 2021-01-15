using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Goat
{
    public class PlayBGMAtDay : AudioCue, IAtomListener<bool>
    {
        [SerializeField] private BoolEvent onCycleChange;
        [SerializeField] private bool atDay;

        private void OnEnable()
        {
            onCycleChange.RegisterSafe(this);
        }

        private void OnDisable()
        {
            onCycleChange.UnregisterSafe(this);
        }

        public void OnEventRaised(bool isDay)
        {
            Debug.Log($"called with {isDay} at {gameObject.name}");
            if (isDay == atDay)
                PlayAudioCue();
            else
                StopAudioCue();
        }
    }
}