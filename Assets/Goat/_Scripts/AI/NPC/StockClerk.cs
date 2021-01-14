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
            // When the customer is standing still for more than a certain amount of time while in moving to a destination
            Func<bool> StuckForSeconds() => () => moveToDestination.timeStuck > 1f || moveToTarget.timeStuck > 1f;
            // Has a target to move to
            Func<bool> HasTarget() => () => targetStorage != null;
            // Has a location to move to
            Func<bool> HasDestination() => () => Vector3.Distance(transform.position, targetDestination) >= npcSize / 2 && targetStorage == null && targetDestination != Vector3.zero;
            // Reached the location of the target
            Func<bool> ReachedTarget() => () => !navMeshAgent.pathPending && navMeshAgent.remainingDistance < npcSize / 2 && targetStorage != null;
            // Reached to position it was moving to
            Func<bool> ReachedDestination() => () => navMeshAgent.remainingDistance < npcSize / 2 && targetStorage == null;
            // When the stockclerk has taken its items to go and fill the shelves in the store
            Func<bool> GoFromTakingToPlacing() => () => takeItem.depleted && targetStorages.Count > 0 && Inventory.ItemsInInventory > 0;
            // When its time to fill the next shelve
            Func<bool> GoFromPlacingToPlacing() => () => placeItem.filled && targetStorages.Count > 0 && Inventory.ItemsInInventory > 0;
            // When stockclerk has entered the store upon opening it can go to work
            Func<bool> EnteredStore() => () => enterStore.enteredStore;
            // When the stockclerk has nothing to do
            Func<bool> NoItemsToTakeOrPlace() => () => Inventory.ItemsInInventory == 0 && ItemsToGet.ItemsInInventory == 0 && placeItem.filled;
            // When the items the stockclerk is looking for are no longer present in the warehouse
            Func<bool> TakenAllItemsFromWarehouse() => () => ItemsToGet.ItemsInInventory == 0 && targetStorages.Count > 0 && Inventory.ItemsInInventory > 0;
            // When the items the stockclerk is looking for are no longer present in the warehouse
            Func<bool> NoItemsFoundInWarehouse() => () => ItemsToGet.ItemsInInventory == 0 && Inventory.ItemsInInventory == 0 && searchForStorageInWarehouse.nothingFound;
            // When there are shelves in the store that need to be filled
            Func<bool> EmptyShelvesFound() => () => searchForEmptyShelves.foundEmptyShelves;
            // When there are no shelves in the store that need to be filled
            Func<bool> NoEmptyShelvesFound() => () => !searchForEmptyShelves.foundEmptyShelves;
            // When the stockclerk still has items to get but cant find them on the shelve he is currently looking at
            Func<bool> FindItemInWarehouse() => () => (ItemsToGet.ItemsInInventory > 0 && takeItem.depleted) || takeItem.depleted;
            // Take items out of storage
            Func<bool> TakeItems() => () => ItemsToGet.ItemsInInventory > 0 && ReachedTarget().Invoke();
            // Place items in storage
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
            AT(takeItem, setStorageTarget, GoFromTakingToPlacing());
            AT(placeItem, setStorageTarget, GoFromPlacingToPlacing());

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