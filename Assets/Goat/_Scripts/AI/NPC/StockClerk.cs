using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI.States;
using Goat.ScriptableObjects;
using Goat.Grid.Interactions;
using UnityAtoms.BaseAtoms;

namespace Goat.AI
{
    public class StockClerk : WorkerWithListener<bool, BoolEvent>
    {
        [SerializeField] private UnloadLocations entrances;


        public override void OnEventRaised(bool value)
        {
            if (!value && stateMachine != null)
                stateMachine.SetState(exitStore);
        }

        protected override void Setup()
        {
            base.Setup();

            EnterStore enterStore = new EnterStore(this, navMeshAgent, entrances);
            SearchForEmptyShelves searchForEmptyShelves = new SearchForEmptyShelves(this);
            SetStorageTarget setStorageTarget = new SetStorageTarget(this);
            SearchForStorageInWarehouse searchForStorageInWarehouse = new SearchForStorageInWarehouse(this);
            SetRandomDestination setRandomDestination = new SetRandomDestination(this, navMeshAgent, 3, null);

            // Conditions
            Func<bool> StuckForSeconds() => () => moveToDestination.timeStuck > 1f || moveToTarget.timeStuck > 1f;
            Func<bool> HasTarget() => () => targetStorage != null;
            Func<bool> HasDestination() => () => Vector3.Distance(transform.position, targetDestination) >= npcSize / 2 && targetStorage == null && targetDestination != Vector3.zero;
            Func<bool> ReachedTarget() => () => !navMeshAgent.pathPending && navMeshAgent.remainingDistance < npcSize / 2 && targetStorage != null;
            Func<bool> ReachedDestination() => () => navMeshAgent.remainingDistance < npcSize / 2 && targetStorage == null;
            Func<bool> SetNextEmptyStorageTarget() => () => placeItem.filled && targetStorages.Count > 0 && Inventory.ItemsInInventory > 0;
            Func<bool> EnteredStore() => () => enterStore.enteredStore;

            Func<bool> NoItemsToTakeOrPlace() => () => Inventory.ItemsInInventory == 0 && ItemsToGet.ItemsInInventory == 0;
            Func<bool> TakenAllItemsFromWarehouse() => () => ItemsToGet.ItemsInInventory == 0 && targetStorages.Count > 0 && Inventory.ItemsInInventory > 0;
            Func<bool> NoItemsFoundInWarehouse() => () => ItemsToGet.ItemsInInventory == 0 && Inventory.ItemsInInventory == 0 && searchForStorageInWarehouse.nothingFound;
            Func<bool> EmptyShelvesFound() => () => searchForEmptyShelves.foundEmptyShelves;
            Func<bool> NoEmptyShelvesFound() => () => !searchForEmptyShelves.foundEmptyShelves;
            Func<bool> FindItemInWarehouse() => () => ItemsToGet.ItemsInInventory > 0 || takeItem.depleted;
            Func<bool> TakeItems() => () => ItemsToGet.ItemsInInventory > 0 && ReachedTarget().Invoke();
            Func<bool> PlaceItems() => () => ItemsToGet.ItemsInInventory == 0 && Inventory.ItemsInInventory > 0 && ReachedTarget().Invoke();

            Func<bool> DoneWaiting() => () => !waitingState.Waiting;

            // Transitions
            void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);

            AT(enterStore, searchForEmptyShelves, EnteredStore());
            AT(placeItem, searchForEmptyShelves, NoItemsToTakeOrPlace());
            AT(searchForEmptyShelves, searchForStorageInWarehouse, EmptyShelvesFound());

            AT(searchForStorageInWarehouse, setStorageTarget, TakenAllItemsFromWarehouse());

            AT(searchForStorageInWarehouse, moveToTarget, HasTarget());
            AT(setStorageTarget, moveToTarget, HasTarget());
            AT(moveToTarget, takeItem, TakeItems());
            AT(moveToTarget, placeItem, PlaceItems());
            //AT(searchForStorageInWarehouse, searchForEmptyShelves, NoItemsFoundInWarehouse());

            AT(takeItem, searchForStorageInWarehouse, FindItemInWarehouse());
            AT(takeItem, setStorageTarget, SetNextEmptyStorageTarget());
            AT(placeItem, setStorageTarget, SetNextEmptyStorageTarget());

            //Nothing to fill found or no items found for filling.
            AT(searchForEmptyShelves, setRandomDestination, NoEmptyShelvesFound());
            AT(setRandomDestination, moveToDestination, HasDestination());
            AT(moveToDestination, waitingState, ReachedDestination());
            AT(waitingState, searchForEmptyShelves, DoneWaiting());
            AT(searchForStorageInWarehouse, setRandomDestination, NoItemsFoundInWarehouse());

            stateMachine.SetState(enterStore);
        }
    }
}