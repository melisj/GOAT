using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using Goat.Grid.Interactions;
using Goat.Storage;
using Goat.AI.States;
using Sirenix.OdinInspector;
using Goat.Pooling;
using Goat.AI.Parking;

namespace Goat.AI
{
    /// <summary>
    /// NPC class which al AI inherret from
    /// </summary>
    public class NPC : SerializedMonoBehaviour, IPoolObject
    {
        // Check variable visability
        public float npcSize = 1f;
        public float wanderRange = 10f;
        [ReadOnly] public int carriedItemValue;
        [SerializeField] private int maxInventory;

        protected StateMachine stateMachine;
        protected MoveToDestination moveToDestination;
        protected MoveToTarget moveToTarget;
        [ReadOnly] public Vector3 targetDestination;

        [ReadOnly] public NavMeshAgent navMeshAgent;
        [ReadOnly] public Animator animator;

        [ReadOnly] public StorageInteractable targetStorage;

        private Inventory itemsToGet;
        public Inventory ItemsToGet => itemsToGet;

        private Inventory inventory;
        public Inventory Inventory => inventory;

        [SerializeField] private string stateName;

        [ReadOnly] public float enterTime;
        public float searchingTime = 0;
        public NPCShip Ship { get; set; }
        public int PoolKey { get; set; }
        public ObjectInstance ObjInstance { get; set; }

        //protected virtual void Awake()
        //{
        //    //awakeTime = Time.time;
        //    //targetDestination = Vector3.one;
        //}

        [HideInInspector] public TakeItem takeItem;

        /// <summary>
        /// Setup which is called everytime the AI is initialized.
        /// </summary>
        protected virtual void Setup()
        {
            stateMachine = new StateMachine();
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();

            inventory = new Inventory(maxInventory);
            itemsToGet = new Inventory(maxInventory);

            moveToDestination = new MoveToDestination(this, navMeshAgent);
            moveToTarget = new MoveToTarget(this, navMeshAgent);
            takeItem = new TakeItem(this, animator, false);
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        /// <summary>
        /// Replace Update with statemachine.Tick so the tick method of a state will be executed every update
        /// </summary>
        protected virtual void Update() => stateMachine.Tick();

        private void LateUpdate()
        {
            stateName = stateMachine?.CurrentState.ToString();
        }

        public virtual void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            ObjInstance = objectInstance;
            PoolKey = poolKey;
            Setup();
        }

        public virtual void OnReturnObject()
        {
            gameObject.SetActive(false);
        }
    }
}