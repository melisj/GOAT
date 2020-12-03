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

        [HideInInspector] public Dictionary<Resource, int> inventory;
        [HideInInspector] public StorageInteractable targetStorage;
        [HideInInspector] public Dictionary<Resource, int> itemsToGet = new Dictionary<Resource, int>();

        protected float maxSearchingTime = 100;
        [HideInInspector] public float enterTime, searchingTime = 0;

        protected virtual void Awake()
        {
            //awakeTime = Time.time;
            targetDestination = transform.position;
            stateMachine = new StateMachine();
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
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

