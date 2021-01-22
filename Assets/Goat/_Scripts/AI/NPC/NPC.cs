using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Goat.Grid.Interactions;
using Goat.Storage;
using Goat.AI.States;
using Sirenix.OdinInspector;
using Goat.Pooling;
using Goat.AI.Parking;
using Sirenix.Serialization;

namespace Goat.AI
{
    /// <summary>
    /// NPC class which al AI inherret from
    /// </summary>
    public class NPC : SerializedMonoBehaviour, IPoolObject
    {
        private const string StateMachinFoldGroup = "StateMachine/Foldout";

        // Check variable visability
        [SerializeField, TabGroup("Settings"), Range(1, 2)] private float npcSize = 1f;
        [SerializeField, TabGroup("Settings"), Range(10, 20)] private float wanderRange = 10f;
        [SerializeField, TabGroup("Settings"), Range(4, 20)] private int maxInventory;
        [SerializeField, TabGroup("References")] private NavMeshAgent navMeshAgent;
        [SerializeField, TabGroup("References")] private Animator animator;

        [TabGroup("StateMachine", "States")]
        [SerializeField, ReadOnly] private string stateName;
        [TabGroup("StateMachine", "States")]
        [SerializeField, ReadOnly] private Vector3 targetDestination;
        [TabGroup("StateMachine", "States")]
        [SerializeField, ReadOnly] private StorageInteractable targetStorage;
        [TabGroup("StateMachine", "States")]
        [SerializeField, ReadOnly] private float enterTime;
        [TabGroup("StateMachine", "States")]
        [SerializeField, ReadOnly] private float searchingTime;

        protected StateMachine stateMachine;
        [SerializeField, HideLabel, TabGroup("StateMachine/States/In", "MoveToDestination")] protected MoveToDestination moveToDestination;
        [SerializeField, HideLabel, TabGroup("StateMachine/States/In", "MoveToTarget")] protected MoveToTarget moveToTarget;
        [SerializeField, HideLabel, TabGroup("StateMachine/States/In", "TakeItem")] private TakeItem takeItem;
        private Inventory itemsToGet;
        public Inventory ItemsToGet => itemsToGet;

        private Inventory inventory;
        public Inventory Inventory => inventory;

        public float EnterTime
        {
            get => enterTime;
            set => enterTime = value;
        }

        public float SearchingTime
        {
            get => searchingTime;
            set => searchingTime = value;
        }

        public NPCShip Ship { get; set; }
        public int PoolKey { get; set; }
        public float NpcSize => npcSize;
        public float WanderRange => wanderRange;
        public ObjectInstance ObjInstance { get; set; }
        public TakeItem TakeItem => takeItem;

        public Vector3 TargetDestination { get => targetDestination; set => targetDestination = value; }
        public StorageInteractable TargetStorage { get => targetStorage; set => targetStorage = value; }
        public NavMeshAgent NavMeshAgent { get => navMeshAgent; set => navMeshAgent = value; }
        public Animator Animator { get => animator; set => animator = value; }

        /// <summary>
        /// Setup which is called everytime the AI is initialized.
        /// </summary>
        protected virtual void Setup()
        {
            stateMachine = new StateMachine();
            NavMeshAgent = GetComponent<NavMeshAgent>();
            Animator = GetComponentInChildren<Animator>();

            inventory = new Inventory(maxInventory);
            itemsToGet = new Inventory(maxInventory);

            moveToDestination = new MoveToDestination(this, NavMeshAgent);
            moveToTarget = new MoveToTarget(this, NavMeshAgent);
            takeItem = new TakeItem(this, Animator, false);
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

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, TargetDestination);
            Gizmos.DrawSphere(TargetDestination, 0.5f);
        }
    }
}