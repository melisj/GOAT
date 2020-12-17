using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI.States;
using Goat.ScriptableObjects;
using Goat.Grid.Interactions;


namespace Goat.AI
{
    public class StockClerk : NPC
    {
        public StorageLocations storageLocations;
        [HideInInspector] public List<StorageInteractable> targetStorages = new List<StorageInteractable>();
        [SerializeField] private UnloadLocations entrances;

        protected override void Setup()
        {
            base.Setup();

            EnterStore enterStore = new EnterStore(this, navMeshAgent, animator, entrances);
            MoveToDestination moveToDestination = new MoveToDestination(this, navMeshAgent, animator);
            TakeItem takeItem = new TakeItem(this, animator, false);
            MoveToTarget moveToTarget = new MoveToTarget(this, navMeshAgent, animator);
            PlaceItem placeItem = new PlaceItem(this, animator);
            SearchForEmptyShelves searchForEmptyShelves = new SearchForEmptyShelves(this);
            SetStorageTarget setStorageTarget = new SetStorageTarget(this);

            // Conditions
            Func<bool> StuckForSeconds() => () => moveToDestination.timeStuck > 1f || moveToTarget.timeStuck > 1f;
            Func<bool> HasTarget() => () => targetStorage != null;
            Func<bool> ReachedTarget() => () => navMeshAgent.remainingDistance < npcSize / 2 && targetStorage != null;
            Func<bool> SetNextEmptyStorageTarget() => () => placeItem.filledShelve && targetStorages.Count > 0;
            Func<bool> EnteredStore() => () => enterStore.enteredStore;

            // Transitions
            void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);

            AT(enterStore, searchForEmptyShelves, EnteredStore());
            //Find containers
            //Take items from containers
            //Set empty target
            //Move to target
            //Place item in target
            //Set empty target
            //Search for empty targets


        }
    }
}

