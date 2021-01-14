using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI.States;
using Goat.ScriptableObjects;
using Goat.Grid.Interactions;


namespace Goat.AI
{
    public class WarehouseWorker : Worker
    {
        [HideInInspector] public bool searching;
        [SerializeField] private ResourceDetection resourceDetecter;

        protected override void Setup()
        {
            base.Setup();

            SearchForStorageInWarehouse searchForStorageInWarehouse = new SearchForStorageInWarehouse(this);
            DoNothing doNothing = new DoNothing(this);
            EnterGoToStorage enterGoToStorage = new EnterGoToStorage(this, navMeshAgent, animator);

            // Conditions
            // Has a location to move to
            Func<bool> HasDestination() => () => resourceDetecter.detected = true && Vector3.Distance(transform.position, targetDestination) > navMeshAgent.radius;
            // Reached to position it was moving to
            Func<bool> ReachedDestination() => () => Vector3.Distance(transform.position, targetDestination) < navMeshAgent.radius && targetStorage == null;
            // Reached the location of the target
            Func<bool> ReachedTarget() => () => Vector3.Distance(transform.position, targetStorage.transform.position) < navMeshAgent.radius * 2 && targetStorage != null;
            // When the warehouseworker has items in its inventory to place in storage
            Func<bool> GoStoreItems() => () => Inventory.SpaceLeft == 0 || Inventory.ItemsInInventory > 0 && !resourceDetecter.detected;
            // Has a target to move to
            Func<bool> HasTarget() => () => targetStorage != null;
            // When all items in inventory are placed inside storage
            Func<bool> EmptiedInventory() => () => Inventory.ItemsInInventory == 0 && placeItem.filled;
            // When there are still items in invetory but the storage is fulll
            Func<bool> FindNextStorage() => () => Inventory.ItemsInInventory > 0 && placeItem.filled;
            // When warehouse worker can start working
            Func<bool> EnteredStorage() => () => enterGoToStorage.entered;
            // When the customer is standing still for more than a certain amount of time while in moving to a destination
            Func<bool> StuckForSeconds() => () => moveToDestination.timeStuck > 1f || moveToTarget.timeStuck > 1f;

            // Transitions
            void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);

            AT(enterGoToStorage, doNothing, EnteredStorage());
            AT(searchForStorageInWarehouse, moveToTarget, HasTarget());
            AT(doNothing, moveToDestination, HasDestination());
            AT(moveToDestination, doNothing, ReachedDestination());
            AT(moveToDestination, doNothing, StuckForSeconds());
            AT(doNothing, searchForStorageInWarehouse, GoStoreItems());
            AT(moveToTarget, placeItem, ReachedTarget());
            AT(placeItem, doNothing, EmptiedInventory());
            AT(placeItem, searchForStorageInWarehouse, FindNextStorage());

            stateMachine.SetState(enterGoToStorage);
        }
    }
}
