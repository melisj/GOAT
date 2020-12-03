using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using Goat.Grid.Interactions;
using Goat.Storage;
using Goat.AI.States;

namespace Goat.AI
{
    public class NPC : MonoBehaviour
    {
        // Check variable visability

        public float npcSize = 1f;
        public float wanderRange = 10f;
        [HideInInspector] public int carriedItemValue;

        protected StateMachine stateMachine;
        [HideInInspector] public Vector3 targetDestination;

        [HideInInspector] public NavMeshAgent navMeshAgent;
        [HideInInspector] public Animator animator;

        [HideInInspector] public Dictionary<Resource, int> Inventory;

        protected virtual void Awake()
        {
            stateMachine = new StateMachine();
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }

        protected virtual void Update() => stateMachine.Tick();
    }
}

