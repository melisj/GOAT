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

namespace Goat.AI
{
    /// <summary>
    /// Worker class which the worker AIs inherrit from
    /// </summary>
    public class Worker : NPC
    {
        [HideInInspector] public bool chillin;
        public StorageList storageLocations;
        [HideInInspector] public PlaceItem placeItem;
        [HideInInspector] protected FindRestingPlace findRestingPlace;
        [HideInInspector] protected WaitingState waitingState;
        protected ExitStore exitStore;

        [HideInInspector] public List<StorageInteractable> targetStorages = new List<StorageInteractable>();

        protected override void Setup()
        {
            base.Setup();

            placeItem = new PlaceItem(this, animator);
            findRestingPlace = new FindRestingPlace(this);
            waitingState = new WaitingState(this, 5);
            exitStore = new ExitStore(this, navMeshAgent);
        }


    }
}

#region
// eigenlijk moet er nog gecheckt worden of een worker items in zijn inventory heeft bij bepaalde stappen
// zoals het wanneer die spullen moet pakken maar al spullen heeft of de winkel uit gaat.
#endregion