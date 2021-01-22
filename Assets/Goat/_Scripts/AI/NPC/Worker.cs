using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.ScriptableObjects;
using Goat.Grid.Interactions;
using Goat.AI.Parking;
using Goat.AI.Satisfaction;
using UnityAtoms.BaseAtoms;
using System;
using Goat.AI.States;
using UnityAtoms;
using ReadOnly = Sirenix.OdinInspector.ReadOnlyAttribute;
using Sirenix.OdinInspector;

namespace Goat.AI
{
    /// <summary>
    /// Worker class which the worker AIs inherrit from
    /// </summary>
    public class Worker : NPC, IAtomListener<bool>
    {
        [SerializeField, TabGroup("Debug"), ReadOnly] private bool chilling;
        [SerializeField, TabGroup("Debug"), ReadOnly] private List<StorageInteractable> targetStorages = new List<StorageInteractable>();
        [SerializeField, TabGroup("Settings"), Range(5, 10)] private float waitingTime;
        [SerializeField, TabGroup("Settings"), Range(0.5f, 2f)] private float placingSpeed;
        [SerializeField, TabGroup("References")] private BoolEvent onCycleChange;
        [SerializeField, TabGroup("References")] private StorageList shelves;
        [SerializeField, TabGroup("References")] private StorageList warehouseStorages;
        [SerializeField, HideLabel, TabGroup("StateMachine/States/In", "PlaceItem")] private PlaceItem placeItem;
        [SerializeField, HideLabel, TabGroup("StateMachine/States/In", "WaitingState")] private WaitingState waitingState;
        [SerializeField, HideLabel, TabGroup("StateMachine/States/In", "ExitStore")] private ExitStore exitStore;

        public PlaceItem PlaceItem => placeItem;
        protected WaitingState WaitingState => waitingState;
        protected ExitStore ExitStore => exitStore;
        public bool Chilling => chilling;
        public StorageList Shelves => shelves;
        public StorageList WarehouseStorages => warehouseStorages;
        public List<StorageInteractable> TargetStorages { get => targetStorages; set => targetStorages = value; }

        protected override void OnEnable()
        {
            base.OnEnable();
            onCycleChange.RegisterSafe(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            onCycleChange.UnregisterSafe(this);
        }

        protected override void Setup()
        {
            base.Setup();

            placeItem = new PlaceItem(this, Animator, placingSpeed);
            waitingState = new WaitingState(this, waitingTime);
            exitStore = new ExitStore(this, NavMeshAgent);
        }

        public void OnEventRaised(bool isDay)
        {
            if (!isDay && stateMachine != null)
                stateMachine.SetState(ExitStore);
        }
    }
}

#region
// eigenlijk moet er nog gecheckt worden of een worker items in zijn inventory heeft bij bepaalde stappen
// zoals het wanneer die spullen moet pakken maar al spullen heeft of de winkel uit gaat.
#endregion