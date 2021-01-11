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

            MoveToDestination moveToDestination = new MoveToDestination(this, navMeshAgent, animator);
            //TakeItem takeItem = new TakeItem(this, animator, false);
            placeItem = new PlaceItem(this, animator);
            MoveToTarget moveToTarget = new MoveToTarget(this, navMeshAgent, animator);
            SearchForStorageInWarehouse searchForStorageInWarehouse = new SearchForStorageInWarehouse(this);
            DoNothing doNothing = new DoNothing(this);
            findRestingPlace = new FindRestingPlace(this);
            waitingState = new WaitingState(this, 5);
            EnterGoToStorage enterGoToStorage = new EnterGoToStorage(this, navMeshAgent, animator);

            // Conditions
            Func<bool> HasDestination() => () => resourceDetecter.detected = true && Vector3.Distance(transform.position, targetDestination) > navMeshAgent.radius;
            Func<bool> ReachedDestination() => () => Vector3.Distance(transform.position, targetDestination) < navMeshAgent.radius && targetStorage == null;
            Func<bool> ReachedTarget() => () => Vector3.Distance(transform.position, targetStorage.transform.position) < navMeshAgent.radius * 2 && targetStorage != null;
            Func<bool> GoStoreItems() => () => Inventory.SpaceLeft == 0 || Inventory.ItemsInInventory > 0 && !resourceDetecter.detected;
            Func<bool> HasTarget() => () => targetStorage != null;
            Func<bool> EmptiedInventory() => () => Inventory.ItemsInInventory == 0 && placeItem.filled;
            Func<bool> FindNextStorage() => () => Inventory.ItemsInInventory > 0 && placeItem.filled;
            Func<bool> EnteredStorage() => () => enterGoToStorage.entered;

            // Transitions
            void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);

            AT(enterGoToStorage, doNothing, EnteredStorage());
            AT(searchForStorageInWarehouse, moveToTarget, HasTarget());
            AT(doNothing, moveToDestination, HasDestination());
            AT(moveToDestination, doNothing, ReachedDestination());
            AT(doNothing, searchForStorageInWarehouse, GoStoreItems());
            AT(moveToTarget, placeItem, ReachedTarget());
            AT(placeItem, doNothing, EmptiedInventory());
            AT(placeItem, searchForStorageInWarehouse, FindNextStorage());

            stateMachine.SetState(enterGoToStorage);
        }
    }
}
