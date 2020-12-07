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

        protected override void Awake()
        {
            base.Awake();

            // States
            CalculateGroceries calculateGroceries = new CalculateGroceries(this, resourcesInProject.Resources);
            EnterStore enterStore = new EnterStore(this, navMeshAgent, animator);
            SetRandomDestination SetRandomDestination = new SetRandomDestination(this);
            MoveToTarget moveToTarget = new MoveToTarget(this, targetDestination, navMeshAgent, animator);
            TakeItem takeItem = new TakeItem(this, animator, false);
            ExitStore exitStore = new ExitStore(this, navMeshAgent, animator);

            // Conditions
            Func<bool> CalculatedGroceries() => () => calculateGroceries.calculatedGroceries;
            Func<bool> EnteredStore() => () => enterStore.enteredStore;
            Func<bool> HasTarget() => () => targetStorage != null && Vector3.Distance(transform.position, targetDestination) >= npcSize;
            Func<bool> HasDestination() => () => Vector3.Distance(transform.position, targetDestination) >= npcSize && targetStorage == null;
            Func<bool> StuckForSeconds() => () =>  moveToTarget.timeStuck > 1f;
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
            AT(SetRandomDestination, moveToTarget, HasTarget());
            AT(SetRandomDestination, moveToTarget, HasDestination());
            AT(moveToTarget, SetRandomDestination, StuckForSeconds());
            AT(moveToTarget, SetRandomDestination, ReachedDestination());
            AT(moveToTarget, takeItem, ReachedTarget());
            AT(takeItem, SetRandomDestination, StorageDepleted());

            stateMachine.SetState(calculateGroceries);
        }

        //protected override void Update() => stateMachine.Tick();

    }
}

