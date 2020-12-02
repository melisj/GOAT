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
    public class Customer : MonoBehaviour
    {
        private StateMachine stateMachine;

        private StorageInteractable targetStorage;

        void Awake()
        {
            //code
            var navMeshAgent = GetComponent<NavMeshAgent>();
            //var animator = GetComponent<Animator>();
            stateMachine = new StateMachine();

            // States

            // Enter store
            // Setup shopping list
            // Search for targets
            // Move to target
            var moveToTarget = new MoveToTarget(gameObject.transform, Vector3.zero, navMeshAgent, new Animator());
            // Take from target
            // Go to checkout
            // Pay
            // Exit store

            // Transitions


            // Functions

            Func<bool> StuckForSeconds() => () => moveToTarget.timeStuck > 1f;
            Func<bool> HasTarget() => () => targetStorage != null;
        }
    }
}

