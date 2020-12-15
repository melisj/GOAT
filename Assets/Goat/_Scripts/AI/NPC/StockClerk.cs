using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI.States;
using Goat.ScriptableObjects;
using Goat.Grid.Interactions;


namespace Goat.AI
{
    public class StockClerk : NPC
    {
        public StorageLocations storageLocations;
        public int maxCarryLoad = 20;
        [HideInInspector] public List<StorageInteractable> targetStorages = new List<StorageInteractable>();

        protected override void Awake()
        {
            base.Awake();

            MoveToDestination moveToDestination = new MoveToDestination(this, navMeshAgent, animator);
            TakeItem takeItem = new TakeItem(this, animator, false);
            MoveToTarget moveToTarget = new MoveToTarget(this, navMeshAgent, animator);

            // Conditions


            // Transitions
            void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
        }
    }
}

