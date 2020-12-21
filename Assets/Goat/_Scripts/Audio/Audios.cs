using UnityEngine;
using System.Collections;
using UnityAtoms.BaseAtoms;
using UnityAtoms;
using System;

namespace Goat.Audio
{
    /// <summary>
    /// Contains the audio to play
    /// </summary>
    public class AudioCue : ScriptableObject
    {
    }

    /// <summary>
    /// Contains the audio settings
    /// </summary>
    public class AudioConfiguration : ScriptableObject
    {
    }

    public interface IDoubleEventListener<T, E>
    {
        void OnEventRaised(T item1, E item2);
    }

    public abstract class AudioEventListener<M, E, C, A> : MonoBehaviour, IDoubleEventListener<M, C> where E : AtomEvent<M> where A : AtomEvent<C>
    {
        [SerializeField] private E musicEvent;
        [SerializeField] private A sfxEvent;

        private IAtomListener<M> musicListener;
        private IAtomListener<C> sfxListener;

        protected E MusicEvent
        {
            get => musicEvent;
            set
            {
                OnDisable();
                musicEvent = value;
                OnEnable();
            }
        }

        protected A SfxEvent
        {
            get => sfxEvent;
            set
            {
                OnDisable();
                sfxEvent = value;
                OnEnable();
            }
        }

        private void OnDisable()
        {
            UnSubFromMusic();
            UnSubFromSfx();
            InitOnDisable();
        }

        private void OnEnable()
        {
            SubToMusic();
            SubToSfx();
            InitOnEnable();
        }

        protected virtual void InitOnEnable()
        {
        }

        protected virtual void InitOnDisable()
        {
        }

        #region Subbing Unsubbing

        private void UnSubFromMusic()
        {
            if (musicEvent == null) return;
            musicEvent.UnregisterListener(musicListener);
        }

        private void UnSubFromSfx()
        {
            if (sfxEvent == null) return;
            sfxEvent.UnregisterListener(sfxListener);
        }

        private void SubToMusic()
        {
            if (musicEvent == null) return;
            musicEvent.RegisterListener(musicListener);
        }

        private void SubToSfx()
        {
            if (sfxEvent == null) return;
            sfxEvent.RegisterListener(sfxListener);
        }

        #endregion Subbing Unsubbing

        public abstract void OnEventRaised(M item1, C item2);
    }
}