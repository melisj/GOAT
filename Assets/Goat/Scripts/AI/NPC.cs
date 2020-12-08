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

        protected StateMachine stateMachine;
        protected MoveToDestination moveToDestination;
        [HideInInspector] public Vector3 targetDestination;

        [HideInInspector] public NavMeshAgent navMeshAgent;
        [HideInInspector] public Animator animator;

        public Dictionary<Resource, int> inventory = new Dictionary<Resource, int>();
        [HideInInspector] public StorageInteractable targetStorage;
        public Dictionary<Resource, int> itemsToGet = new Dictionary<Resource, int>();

        protected float maxSearchingTime = 5;
        [HideInInspector] public float enterTime;
        public float searchingTime = 0;

        protected virtual void Awake()
        {
            //awakeTime = Time.time;
            //targetDestination = Vector3.one;
            stateMachine = new StateMachine();
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();

            MoveToDestination moveToDestination = new MoveToDestination(this, navMeshAgent, animator);

        }

        protected virtual void Update() => stateMachine.Tick();

        public void AddResourceToInventory(Resource resource, int amount)
        {
            if (inventory.ContainsKey(resource))
                inventory[resource] += amount;
            else
                inventory.Add(resource, amount);
        }
        public void RemoveResourceFromInventory(Resource resource, int amount)
        {
            if (inventory.ContainsKey(resource))
            {
                inventory[resource] -= amount;
                if (inventory[resource] <= 0)
                    inventory.Remove(resource);
            }
        }
        public void RemoveItemToGet(Resource resource, int amount)
        {
            if (itemsToGet.ContainsKey(resource))
            {
                itemsToGet[resource] -= amount;
                if (itemsToGet[resource] <= 0)
                    itemsToGet.Remove(resource);
            }
        }
    }
}

