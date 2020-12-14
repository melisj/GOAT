﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Goat.AI.States;
using Goat.Storage;
using Goat.Grid.Interactions;

namespace Goat.AI
{
    public class Customer : NPC
    {
        // Choosen for player money instead of grocery amount because money gives a more dynamic way of handeling groceries and buying behaviour.
        [SerializeField] private float maxSearchingTime = 60;
        public int money = 0;
        [HideInInspector] public int remainingMoney = 0;

        [SerializeField] private ResourceArray resourcesInProject;

        public float customerSatisfaction = 100;
        [HideInInspector] public float customerSelfConstraint = 0;
        [SerializeField] private FieldOfView fov;
        [HideInInspector] public bool enteredStore;
        [HideInInspector] public bool leavingStore;

        [HideInInspector] public float totalPriceProducts;

        ExitStore exitStore;

        //[HideInInspector] public WaitAt
        protected override void Awake()
        {
            base.Awake();

            // States
            CalculateGroceries calculateGroceries = new CalculateGroceries(this, resourcesInProject.Resources);
            EnterStore enterStore = new EnterStore(this, navMeshAgent, animator);
            SetRandomDestination SetRandomDestination = new SetRandomDestination(this, navMeshAgent);
            moveToDestination = new MoveToDestination(this, navMeshAgent, animator);
            MoveToTarget moveToTarget = new MoveToTarget(this, navMeshAgent, animator);
            TakeItem takeItem = new TakeItem(this, animator, false);
            SearchForCheckout searchForCheckout = new SearchForCheckout(this);
            exitStore = new ExitStore(this, navMeshAgent, animator);
            DoNothing doNothing = new DoNothing(this);

            // Conditions
            // Groceries
            Func<bool> CalculatedGroceries() => () => calculateGroceries.calculatedGroceries;
            Func<bool> EnteredStore() => () => enterStore.enteredStore;
            // Movement
            Func<bool> HasStorageTarget() => () => targetStorage != null;
            Func<bool> HasDestination() => () => Vector3.Distance(transform.position, targetDestination) >= npcSize / 2 && targetStorage == null;
            Func<bool> StuckForSeconds() => () =>  moveToDestination.timeStuck > 1f || moveToTarget.timeStuck > 1f;
            Func<bool> ReachedDestination() => () => navMeshAgent.remainingDistance < npcSize/2 &&  targetStorage == null && !searchForCheckout.inQueue;
            Func<bool> ReachedTarget() => () => navMeshAgent.remainingDistance < npcSize / 2 && targetStorage != null;
            // Shopping
            Func<bool> StorageDepleted() => () => takeItem.storageDepleted;
            Func<bool> GoToCheckout() => () => (searchingTime >= maxSearchingTime || (itemsToGet.Count == 0 && enterStore.enteredStore)) && searchForCheckout.checks < 1;
            Func<bool> FindShortestCheckoutQueue() => () => navMeshAgent.remainingDistance < 4 && (searchForCheckout.checks < 2 && searchForCheckout.checks > 0);
            //Func<bool> ArrivedAtCheckout() => () => itemsToGet.Count == 0 && Vector3.Distance(transform.position, targetDestination) < npcSize && targetStorage == null;
            // Interaction
            Func<bool> AskForHelp() => () => itemsToGet.Count > 0 && searchingTime >= maxSearchingTime;
            // Checkout
            Func<bool> WaitingInQueue() => () => searchForCheckout.inQueue && navMeshAgent.remainingDistance < 0.1f;

            // Transitions
            void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);

            AT(calculateGroceries, enterStore, CalculatedGroceries());
            AT(enterStore, SetRandomDestination, EnteredStore());
            //AT(SetRandomDestination, moveToTarget, HasTarget());
            AT(SetRandomDestination, moveToDestination, HasDestination());
            AT(moveToDestination, SetRandomDestination, StuckForSeconds());
            AT(moveToDestination, SetRandomDestination, ReachedDestination());
            AT(moveToTarget, takeItem, ReachedTarget());
            AT(takeItem, SetRandomDestination, StorageDepleted());
            AT(moveToDestination, moveToTarget, HasStorageTarget());
            AT(SetRandomDestination, moveToTarget, HasStorageTarget());

            AT(moveToDestination, searchForCheckout, GoToCheckout());
            AT(searchForCheckout, moveToDestination, HasDestination());
            AT(moveToDestination, searchForCheckout, FindShortestCheckoutQueue());

            AT(moveToDestination, doNothing, WaitingInQueue());

            stateMachine.SetState(calculateGroceries);

            fov = GetComponentInChildren<FieldOfView>();
        }

        public void UpdatePositionInCheckoutQueue(Vector3 newPosition)
        {
            targetDestination = newPosition;
            stateMachine.SetState(moveToDestination);
        }
        public void LeaveStore()
        {
            stateMachine.SetState(exitStore);
        }
    }
}
