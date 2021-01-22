using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI.States;
using Goat.ScriptableObjects;
using Goat.Grid.Interactions;
using Sirenix.OdinInspector;

namespace Goat.AI
{
    public class WarehouseWorker : Worker
    {
        [SerializeField, TabGroup("Debug"), ReadOnly] private bool searching;
        [SerializeField, TabGroup("References")] private ResourceDetection resourceDetecter;

        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool hasDestination;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool reachedDestination;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool reachedTarget;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool goStoreItems;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool hasTarget;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool emptiedInventory;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool findNextStorage;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool enteredStorage;
        [SerializeField, ReadOnly, TabGroup("StateMachine", "Conditions")] private bool stuckForSeconds;

        public bool Searching { get => searching; set => searching = value; }

        protected override void Setup()
        {
            base.Setup();

            SearchForStorageInWarehouse searchForStorageInWarehouse = new SearchForStorageInWarehouse(this);
            DoNothing doNothing = new DoNothing(this);
            EnterGoToStorage enterGoToStorage = new EnterGoToStorage(this, NavMeshAgent, Animator);

            // Conditions
            // Has a location to move to
            Func<bool> HasDestination() => () =>
            {
                hasDestination = resourceDetecter.detected = true && Vector3.Distance(transform.position, TargetDestination) > NavMeshAgent.radius;
                return hasDestination;
            };
            // Reached to position it was moving to
            Func<bool> ReachedDestination() => () =>
            {
                reachedDestination = Vector3.Distance(transform.position, TargetDestination) < NavMeshAgent.radius && TargetStorage == null;
                return reachedDestination;
            };
            // Reached the location of the target
            Func<bool> ReachedTarget() => () =>
            {
                reachedTarget = Vector3.Distance(transform.position, TargetStorage.transform.position) < NavMeshAgent.radius * 2 && TargetStorage != null;
                return reachedTarget;
            };
            // When the warehouseworker has items in its inventory to place in storage
            Func<bool> GoStoreItems() => () =>
            {
                goStoreItems = Inventory.SpaceLeft == 0 || Inventory.ItemsInInventory > 0 && !resourceDetecter.detected;
                return goStoreItems;
            };
            // Has a target to move to
            Func<bool> HasTarget() => () =>
            {
                hasTarget = TargetStorage != null;
                return hasTarget;
            };
            // When all items in inventory are placed inside storage
            Func<bool> EmptiedInventory() => () =>
            {
                emptiedInventory = Inventory.ItemsInInventory == 0 && PlaceItem.Filled;
                return emptiedInventory;
            };
            // When there are still items in inventory but the storage is full
            Func<bool> FindNextStorage() => () =>
            {
                findNextStorage = Inventory.ItemsInInventory > 0 && PlaceItem.Filled;
                return findNextStorage;
            };
            // When warehouse worker can start working
            Func<bool> EnteredStorage() => () =>
            {
                enteredStorage = enterGoToStorage.entered;
                return enteredStorage;
            };
            // When the customer is standing still for more than a certain amount of time while in moving to a destination
            Func<bool> StuckForSeconds() => () =>
            {
                stuckForSeconds = moveToDestination.TimeStuck > 1f || moveToTarget.TimeStuck > 1f;
                return stuckForSeconds;
            };

            // Transitions
            void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);

            AT(enterGoToStorage, doNothing, EnteredStorage());
            AT(searchForStorageInWarehouse, moveToTarget, HasTarget());
            AT(doNothing, moveToDestination, HasDestination());
            AT(moveToDestination, doNothing, ReachedDestination());
            AT(moveToDestination, doNothing, StuckForSeconds());
            AT(doNothing, searchForStorageInWarehouse, GoStoreItems());
            AT(moveToTarget, PlaceItem, ReachedTarget());
            AT(PlaceItem, doNothing, EmptiedInventory());
            AT(PlaceItem, searchForStorageInWarehouse, FindNextStorage());

            stateMachine.SetState(enterGoToStorage);
        }
    }
}