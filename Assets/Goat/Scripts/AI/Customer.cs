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

        protected override void Awake()
        {
            base.Awake();

            // States
            CalculateGroceries calculateGroceries = new CalculateGroceries(this, resourcesInProject.Resources);
            SearchForGroceries searchForGroceries = new SearchForGroceries(this);
            MoveToTarget moveToTarget = new MoveToTarget(this, targetDestination, navMeshAgent, animator);
            TakeItem takeItem = new TakeItem(this, targetStorage, null, false);

            // Conditions
            Func<bool> CalculatedGroceries() => () => calculateGroceries.calculatedGroceries;
            Func<bool> HasTarget() => () => targetStorage != null && Vector3.Distance(transform.position, targetDestination) >= npcSize;
            Func<bool> HasDestination() => () => Vector3.Distance(transform.position, targetDestination) >= npcSize && targetStorage == null;
            Func<bool> StuckForSeconds() => () =>  moveToTarget.timeStuck > 1f;
            Func<bool> ReachedDestination() => () => Vector3.Distance(transform.position, targetDestination) < npcSize && targetStorage == null;
            Func<bool> StorageDepleted() => () => takeItem.storageDepleted;
            Func<bool> GoToCheckout() => () => itemsToGet.Count == 0;
            Func<bool> ArrivedAtCheckout() => () => itemsToGet.Count == 0 && Vector3.Distance(transform.position, targetDestination) < npcSize && targetStorage == null;

            // Transitions
            void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(to, from, condition);

            AT(calculateGroceries, searchForGroceries, CalculatedGroceries());
            AT(searchForGroceries, moveToTarget, HasTarget());
            AT(searchForGroceries, moveToTarget, HasDestination());
            AT(moveToTarget, searchForGroceries, StuckForSeconds());
            AT(moveToTarget, searchForGroceries, ReachedDestination());
            AT(takeItem, searchForGroceries, StorageDepleted());
        }
    }
}

