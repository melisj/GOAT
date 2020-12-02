using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms;
using UnityAtoms.BaseAtoms;

namespace Goat.Events
{
    public abstract class EventListener<T, E> : MonoBehaviour, UnityAtoms.IAtomListener<T> where E : AtomEvent<T>
    {
        [SerializeField] private E subscribedEvent = null;

        protected E SubcribedEvent
        {
            get => subscribedEvent;
            set
            {
                OnDisable();
                subscribedEvent = value;
                OnEnable();
            }
        }

        private void Awake()
        {
            OnEnable();
        }

        private void OnEnable()
        {
            if (subscribedEvent == null) return;
            subscribedEvent.RegisterListener(this);
            InitOnEnable();
        }

        private void OnDisable()
        {
            if (subscribedEvent == null) return;
            subscribedEvent.UnregisterListener(this);
        }

        public abstract void OnEventRaised(T value);

        protected virtual void InitOnEnable()
        {
        }
    }

    public abstract class EventListenerInt : EventListener<int, IntEvent> { }

    public abstract class EventListenerFloat : EventListener<float, FloatEvent> { }

    public abstract class EventListenerBool : EventListener<bool, BoolEvent> { }

    public abstract class EventListenerString : EventListener<string, StringEvent> { }

    public abstract class EventListenerInputMode : EventListener<InputMode, InputModeEvent> { }
}