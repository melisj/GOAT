using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI.States;
using Goat.ScriptableObjects;
using Goat.Grid.Interactions;


namespace Goat.AI
{
    public class StockClerk : Worker
    {

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
            SearchForStorageInWarehouse searchForStorageInWarehouse = new SearchForStorageInWarehouse(this);

            // Conditions
            Func<bool> StuckForSeconds() => () => moveToDestination.timeStuck > 1f || moveToTarget.timeStuck > 1f;
            Func<bool> HasTarget() => () => targetStorage != null;
            Func<bool> ReachedTarget() => () => navMeshAgent.remainingDistance < npcSize / 2 && targetStorage != null;
            Func<bool> SetNextEmptyStorageTarget() => () => placeItem.filledShelve && targetStorages.Count > 0 && Inventory.ItemsInInventory > 0;
            Func<bool> EnteredStore() => () => enterStore.enteredStore;

            Func<bool> NoItemsToTakeOrPlace() => () => Inventory.ItemsInInventory == 0 && ItemsToGet.ItemsInInventory == 0 && targetStorages.Count == 0;
            Func<bool> FindItemInWarehouse() => () => ItemsToGet.ItemsInInventory > 0 && !placeItem.filledShelve;
            Func<bool> TakeItems() => () => ItemsToGet.ItemsInInventory > 0 && ReachedTarget().Invoke();
            Func<bool> PlaceItems() => () => ItemsToGet.ItemsInInventory == 0 && Inventory.ItemsInInventory > 0 && ReachedTarget().Invoke();

            // Transitions
            void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);

            AT(enterStore, searchForEmptyShelves, EnteredStore());
            AT(placeItem, searchForEmptyShelves, NoItemsToTakeOrPlace());
            AT(searchForEmptyShelves, searchForStorageInWarehouse, FindItemInWarehouse());

            AT(searchForStorageInWarehouse, moveToTarget, HasTarget());
            AT(setStorageTarget, moveToTarget, HasTarget());
            AT(moveToTarget, takeItem, TakeItems());
            AT(moveToTarget, placeItem, PlaceItems());

            AT(takeItem, searchForStorageInWarehouse, FindItemInWarehouse());
            AT(takeItem, setStorageTarget, SetNextEmptyStorageTarget());
            AT(placeItem, setStorageTarget, SetNextEmptyStorageTarget());
            AT(placeItem, searchForEmptyShelves, NoItemsToTakeOrPlace());
        }
    }
}

