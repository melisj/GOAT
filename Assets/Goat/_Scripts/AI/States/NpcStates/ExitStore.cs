using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Goat.Pooling;
using Sirenix.OdinInspector;

namespace Goat.AI.States
{
    /// <summary>
    /// Exit the store
    /// </summary>
    [System.Serializable]
    public class ExitStore : IState
    {
        protected NPC npc;
        protected NavMeshAgent navMeshAgent;
        [SerializeField, ReadOnly] protected Vector3 entrance;
        [SerializeField, ReadOnly] protected float destinationDistance;
        [SerializeField, ReadOnly] private bool exitedStore;
        public bool ExitedStore => exitedStore;

        public ExitStore(NPC npc, NavMeshAgent navMeshAgent)
        {
            this.npc = npc;
            this.navMeshAgent = navMeshAgent;
        }

        public virtual void Tick()
        {
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < npc.NpcSize / 2)
            {
                OnExit();
            }
        }

        public virtual void OnEnter()
        {
            exitedStore = false;

            entrance = npc.Ship.NpcSpawner.transform.position;
            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(entrance);
        }

        public virtual void OnExit()
        {
            npc.Ship.ShipReadyToFly();
            navMeshAgent.ResetPath();
            PoolManager.Instance.ReturnToPool(npc.gameObject);
        }
    }
}