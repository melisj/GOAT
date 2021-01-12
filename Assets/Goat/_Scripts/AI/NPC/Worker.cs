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
    public class Worker : NPC
    {
        [HideInInspector] public bool chillin;
        public StorageList storageLocations;
        [HideInInspector] public PlaceItem placeItem;
        [HideInInspector] protected FindRestingPlace findRestingPlace;
        [HideInInspector] protected WaitingState waitingState;
        protected ExitStore exitStore;

        [HideInInspector] public List<StorageInteractable> targetStorages = new List<StorageInteractable>();


    }
}