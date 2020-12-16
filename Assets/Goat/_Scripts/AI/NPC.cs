﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using Goat.Grid.Interactions;
using Goat.Storage;
using Goat.AI.States;
using Sirenix.OdinInspector;

namespace Goat.AI
{
    public class NPC : SerializedMonoBehaviour
    {
        // Check variable visability
        public float npcSize = 1f;
        public float wanderRange = 10f;
        [HideInInspector] public int carriedItemValue;
        [SerializeField] private int maxInventory;

        protected StateMachine stateMachine;
        protected MoveToDestination moveToDestination;
        [HideInInspector] public Vector3 targetDestination;

        [HideInInspector] public NavMeshAgent navMeshAgent;
        [HideInInspector] public Animator animator;

        [HideInInspector] public StorageInteractable targetStorage;

        private Inventory itemsToGet;
        public Inventory ItemsToGet => itemsToGet;

        private Inventory inventory;
        public Inventory Inventory => inventory;

        [HideInInspector] public float enterTime;
        public float searchingTime = 0;

        protected virtual void Awake()
        {
            //awakeTime = Time.time;
            //targetDestination = Vector3.one;
            stateMachine = new StateMachine();
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();

            inventory = new Inventory(maxInventory);
            itemsToGet = new Inventory(maxInventory);

            MoveToDestination moveToDestination = new MoveToDestination(this, navMeshAgent, animator);

        }

        protected virtual void Update() => stateMachine.Tick();
    }
}

