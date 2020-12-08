using System;
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
        public int money = 0;
        [HideInInspector] public int remainingMoney = 0;

        [SerializeField] private ResourceArray resourcesInProject;

        public float customerSatisfaction = 100;
        [HideInInspector] public float customerSelfConstraint = 0;
        [SerializeField] private FieldOfView fov;
        [HideInInspector] public bool enteredStore;

        protected override void Awake()
        {
            base.Awake();

            // States
            CalculateGroceries calculateGroceries = new CalculateGroceries(this, resourcesInProject.Resources);
            EnterStore enterStore = new EnterStore(this, navMeshAgent, animator);
            SetRandomDestination SetRandomDestination = new SetRandomDestination(this);
            MoveToDestination moveToDestination = new MoveToDestination(this, navMeshAgent, animator);
            MoveToTarget moveToTarget = new MoveToTarget(this, navMeshAgent, animator);
            TakeItem takeItem = new TakeItem(this, animator, false);
            ExitStore exitStore = new ExitStore(this, navMeshAgent, animator);

            // Conditions
            Func<bool> CalculatedGroceries() => () => calculateGroceries.calculatedGroceries;
            Func<bool> EnteredStore() => () => enterStore.enteredStore;
            Func<bool> HasTarget() => () => targetStorage != null;
            Func<bool> HasDestination() => () => Vector3.Distance(transform.position, targetDestination) >= npcSize && targetStorage == null;
            Func<bool> StuckForSeconds() => () =>  moveToDestination.timeStuck > 1f || moveToTarget.timeStuck > 1f;
            Func<bool> ReachedDestination() => () => Vector3.Distance(transform.position, targetDestination) < npcSize &&  targetStorage == null;
            Func<bool> ReachedTarget() => () => Vector3.Distance(transform.position, targetDestination) < npcSize && targetStorage != null;
            Func<bool> StorageDepleted() => () => takeItem.storageDepleted;
            Func<bool> GoToCheckout() => () => itemsToGet.Count == 0;
            Func<bool> ArrivedAtCheckout() => () => itemsToGet.Count == 0 && Vector3.Distance(transform.position, targetDestination) < npcSize && targetStorage == null;
            Func<bool> AskForHelp() => () => itemsToGet.Count > 0 && searchingTime >= maxSearchingTime;

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
            AT(moveToDestination, moveToTarget, HasTarget());

            stateMachine.SetState(calculateGroceries);

            fov = GetComponentInChildren<FieldOfView>();
        }

        //protected override void Update() => stateMachine.Tick();

    }
}

