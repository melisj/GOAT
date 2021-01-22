using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI.States;
using Goat.ScriptableObjects;
using Goat.Grid.Interactions;
using UnityAtoms.BaseAtoms;
using Sirenix.OdinInspector;

namespace Goat.AI
{
    public class StockClerk : Worker
    {
        [SerializeField, TabGroup("References")] private UnloadLocations entrances;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool stuckForSeconds;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool hasTarget;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool hasDestination;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool reachedTarget;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool reachedDestination;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool goFromTakingToPlacing;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool goFromPlacingToPlacing;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool enteredStore;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool noItemsToTakeOrPlace;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool takenAllItemsFromWarehouse;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool noItemsFoundInWarehouse;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool emptyShelvesFound;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool noEmptyShelvesFound;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool findItemInWarehouse;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool takeItems;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool placeItems;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool doneWaiting;

        protected override void Setup()
        {
            base.Setup();

            EnterStore enterStore = new EnterStore(this, NavMeshAgent, entrances);
            SearchForEmptyShelves searchForEmptyShelves = new SearchForEmptyShelves(this);
            SetStorageTarget setStorageTarget = new SetStorageTarget(this);
            SearchForStorageInWarehouse searchForStorageInWarehouse = new SearchForStorageInWarehouse(this);
            SetRandomDestination setRandomDestination = new SetRandomDestination(this, NavMeshAgent, 3);

            // Conditions
            // When the customer is standing still for more than a certain amount of time while in moving to a destination
            Func<bool> StuckForSeconds() => () =>
            {
                stuckForSeconds = moveToDestination.TimeStuck > 1f || moveToTarget.TimeStuck > 1f;
                return stuckForSeconds;
            };
            // Has a target to move to
            Func<bool> HasTarget() => () =>
            {
                hasTarget = TargetStorage != null;
                return hasTarget;
            };
            // Has a location to move to
            Func<bool> HasDestination() => () =>
            {
                hasDestination = Vector3.Distance(transform.position, TargetDestination) >= NpcSize / 2 && TargetStorage == null && TargetDestination != Vector3.zero;
                return hasDestination;
            };
            // Reached the location of the target
            Func<bool> ReachedTarget() => () =>
            {
                reachedTarget = !NavMeshAgent.pathPending && NavMeshAgent.remainingDistance < NpcSize / 2 && TargetStorage != null;
                return reachedTarget;
            };
            // Reached to position it was moving to
            Func<bool> ReachedDestination() => () =>
            {
                reachedDestination = NavMeshAgent.remainingDistance < NpcSize / 2 && TargetStorage == null;
                return reachedDestination;
            };
            // When the stockclerk has taken its items to go and fill the shelves in the store
            Func<bool> GoFromTakingToPlacing() => () =>
            {
                goFromTakingToPlacing = TakeItem.Depleted && TargetStorages.Count > 0 && Inventory.ItemsInInventory > 0;
                return goFromTakingToPlacing;
            };
            // When its time to fill the next shelve
            Func<bool> GoFromPlacingToPlacing() => () =>
            {
                goFromPlacingToPlacing = PlaceItem.Filled && TargetStorages.Count > 0 && Inventory.ItemsInInventory > 0;
                return goFromPlacingToPlacing;
            };
            // When stockclerk has entered the store upon opening it can go to work
            Func<bool> EnteredStore() => () =>
            {
                enteredStore = enterStore.enteredStore;
                return enteredStore;
            };
            // When the stockclerk has nothing to do
            Func<bool> NoItemsToTakeOrPlace() => () =>
            {
                noItemsToTakeOrPlace = Inventory.ItemsInInventory == 0 && ItemsToGet.ItemsInInventory == 0 && PlaceItem.Filled;
                return noItemsToTakeOrPlace;
            };
            // When the items the stockclerk is looking for are no longer present in the warehouse
            Func<bool> TakenAllItemsFromWarehouse() => () =>
            {
                takenAllItemsFromWarehouse = ItemsToGet.ItemsInInventory == 0 && TargetStorages.Count > 0 && Inventory.ItemsInInventory > 0;
                return takenAllItemsFromWarehouse;
            };
            // When the items the stockclerk is looking for are no longer present in the warehouse
            Func<bool> NoItemsFoundInWarehouse() => () =>
            {
                noItemsFoundInWarehouse = ItemsToGet.ItemsInInventory == 0 && Inventory.ItemsInInventory == 0 && searchForStorageInWarehouse.nothingFound;
                return noItemsFoundInWarehouse;
            };
            // When there are shelves in the store that need to be filled
            Func<bool> EmptyShelvesFound() => () =>
            {
                emptyShelvesFound = searchForEmptyShelves.foundEmptyShelves;
                return emptyShelvesFound;
            };
            // When there are no shelves in the store that need to be filled
            Func<bool> NoEmptyShelvesFound() => () =>
            {
                noEmptyShelvesFound = !searchForEmptyShelves.foundEmptyShelves;
                return noEmptyShelvesFound;
            };
            // When the stockclerk still has items to get but cant find them on the shelve he is currently looking at
            Func<bool> FindItemInWarehouse() => () =>
            {
                findItemInWarehouse = (ItemsToGet.ItemsInInventory > 0 && TakeItem.Depleted) || TakeItem.Depleted;
                return findItemInWarehouse;
            };
            // Take items out of storage
            Func<bool> TakeItems() => () =>
            {
                takeItems = ItemsToGet.ItemsInInventory > 0 && ReachedTarget().Invoke();
                return takeItems;
            };
            // Place items in storage
            Func<bool> PlaceItems() => () =>
            {
                placeItems = ItemsToGet.ItemsInInventory == 0 && Inventory.ItemsInInventory > 0 && ReachedTarget().Invoke();
                return placeItems;
            };

            Func<bool> DoneWaiting() => () =>
            {
                doneWaiting = !WaitingState.Waiting;
                return doneWaiting;
            };

            // Transitions
            void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);

            AT(enterStore, searchForEmptyShelves, EnteredStore());
            AT(PlaceItem, searchForEmptyShelves, NoItemsToTakeOrPlace());
            AT(searchForEmptyShelves, searchForStorageInWarehouse, EmptyShelvesFound());

            AT(searchForStorageInWarehouse, setStorageTarget, TakenAllItemsFromWarehouse());

            AT(searchForStorageInWarehouse, moveToTarget, HasTarget());
            AT(setStorageTarget, moveToTarget, HasTarget());
            AT(moveToTarget, TakeItem, TakeItems());
            AT(moveToTarget, PlaceItem, PlaceItems());
            //AT(searchForStorageInWarehouse, searchForEmptyShelves, NoItemsFoundInWarehouse());

            AT(TakeItem, searchForStorageInWarehouse, FindItemInWarehouse());
            AT(TakeItem, setStorageTarget, GoFromTakingToPlacing());
            AT(PlaceItem, setStorageTarget, GoFromPlacingToPlacing());

            //Nothing to fill found or no items found for filling.
            AT(searchForEmptyShelves, setRandomDestination, NoEmptyShelvesFound());
            AT(setRandomDestination, moveToDestination, HasDestination());
            AT(moveToDestination, WaitingState, ReachedDestination());
            AT(WaitingState, searchForEmptyShelves, DoneWaiting());
            AT(searchForStorageInWarehouse, setRandomDestination, NoItemsFoundInWarehouse());

            stateMachine.SetState(enterStore);
        }
    }
}