using Goat.Events;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Goat.Audio
{
    public class PlayAudioOnGridEdit : AudioCue, IAtomListener<UnityAtoms.Void>
    {
        [SerializeField] private VoidEvent onGridEdit;

        private void OnEnable()
        {
            onGridEdit.RegisterSafe(this);
        }

        private void OnDisable()
        {
            onGridEdit.UnregisterSafe(this);
        }

        public void OnEventRaised(Void item)
        {
            PlayAudioCue();
        }
    }
}