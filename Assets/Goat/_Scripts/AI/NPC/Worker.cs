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

namespace Goat.AI
{
    /// <summary>
    /// Worker class which the worker AIs inherrit from
    /// </summary>
    public class Worker : NPC, IAtomListener<bool>
    {
        [HideInInspector] public bool chillin;
        [SerializeField] private BoolEvent onCycleChange;
        public StorageList storageLocations;
        [HideInInspector] public PlaceItem placeItem;
        [HideInInspector] protected WaitingState waitingState;
        protected ExitStore exitStore;

        [HideInInspector] public List<StorageInteractable> targetStorages = new List<StorageInteractable>();

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

            placeItem = new PlaceItem(this, animator);
            waitingState = new WaitingState(this, 5);
            exitStore = new ExitStore(this, navMeshAgent);
        }

        public void OnEventRaised(bool isDay)
        {
            if (!isDay && stateMachine != null)
                stateMachine.SetState(exitStore);
        }
    }
}

#region
// eigenlijk moet er nog gecheckt worden of een worker items in zijn inventory heeft bij bepaalde stappen
// zoals het wanneer die spullen moet pakken maar al spullen heeft of de winkel uit gaat.
#endregion