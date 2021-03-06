﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using Goat.Grid.Interactions;
using Sirenix.OdinInspector;
using Goat.Grid;
using Goat.AI.Parking;
using Goat.Expenses;

namespace Goat.Events
{
    public abstract class EventListener<T, E> : SerializedMonoBehaviour, UnityAtoms.IAtomListener<T> where E : AtomEvent<T>
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

        private void Awake()
        {
            //  OnEnable();
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

    public abstract class EventListenerInt : EventListener<int, IntEvent> { }

    public abstract class EventListenerVoid : EventListener<Void, VoidEvent> { }

    public abstract class EventListenerVector3 : EventListener<Vector3, Vector3Event> { }

    public abstract class EventListenerVector3WithOwner : EventListener<WithOwner<Vector3>, Vector3OwnerEvent> { }

    public abstract class EventListenerVector3HashSetEvent : EventListener<WithOwner<HashSet<Vector3>>, Vector3HashSetEvent> { }

    public abstract class EventListenerFloat : EventListener<float, FloatEvent> { }

    public abstract class EventListenerBool : EventListener<bool, BoolEvent> { }

    public abstract class EventListenerString : EventListener<string, StringEvent> { }

    public abstract class EventListenerInputMode : EventListener<InputMode, InputModeEvent> { }

    public abstract class EventListenerKeyCodeModeEvent : EventListener<KeyCodeMode, KeyCodeModeEvent> { }

    public abstract class EventListenerPlaceable : EventListener<Placeable, PlaceableEvent> { }

    public abstract class EventListenerReview : EventListener<Review, ReviewEvent> { }

    public abstract class EventListenerDeliveryResource : EventListener<DeliveryResource, DeliveryResourceEvent> { }

    public abstract class EventListenerHiredEmployee : EventListener<HiredEmployee, HiredEmployeeEvent> { }

    public abstract class EventListenerExpenseEvent : EventListener<Expense, ExpenseEvent> { }

    public abstract class EventListenerInteractable : EventListener<BaseInteractable, InteractableEvent> { }
}