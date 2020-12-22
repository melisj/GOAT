using UnityEngine;
using UnityAtoms;

namespace Goat.AI
{
    public abstract class WorkerWithListener<T, E> : Worker, UnityAtoms.IAtomListener<T> where E : AtomEvent<T>
    {
        [SerializeField] private E subscribedEvent = null;
        [SerializeField, UnityAtoms.ReadOnly] protected int subscribers;
        public int Subscribers => subscribers;

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

        private void OnEnable()
        {
            if (subscribedEvent == null) return;
            subscribedEvent.RegisterListener(this);
            subscribers++;
            InitOnEnable();
        }

        private void OnDisable()
        {
            if (subscribedEvent == null) return;
            subscribedEvent.UnregisterListener(this);
            subscribers--;
            InitOnDisable();
        }

        public abstract void OnEventRaised(T value);

        protected virtual void InitOnEnable()
        {
        }

        protected virtual void InitOnDisable()
        {
        }
    }
}