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
        public float size = 1, wanderRange = 10;
        // Choosen for player money instead of grocery amount because money gives a more dynamic way of handeling groceries and buying behaviour.
        public int money = 0;
        [HideInInspector] public int remainingMoney = 0;

        [HideInInspector] public Dictionary<Resource, int> groceries = new Dictionary<Resource, int>();
        [HideInInspector] public StorageInteractable targetStorage;

        protected override void Awake()
        {
            base.Awake();

            // States
            CalculateGroceries calculateGroceries = new CalculateGroceries(this);
            SearchForGroceries searchForGroceries = new SearchForGroceries(this);
            MoveToTarget moveToTarget = new MoveToTarget(this, targetDestination, navMeshAgent, animator);

            // Conditions
            Func<bool> CalculatedGroceries() => () => calculateGroceries.calculatedGroceries;
            Func<bool> HasTarget() => () => targetStorage != null;
            Func<bool> HasDestination() => () => Vector3.Distance(transform.position, targetDestination) >= size;
            Func<bool> StuckForSeconds() => () =>  moveToTarget.timeStuck > 1f;
            Func<bool> ReachedDestination() => () => Vector3.Distance(transform.position, targetDestination) < size;

            // Transitions
        }
    }
}

