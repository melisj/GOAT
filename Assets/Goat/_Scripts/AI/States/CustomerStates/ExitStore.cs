using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Goat.Pooling;

namespace Goat.AI.States
{
    public class ExitStore : IState
    {
        protected NPC npc;
        protected NavMeshAgent navMeshAgent;
        public bool exitedStore;
        protected Vector3 entrance;
        protected float destinationDistance;

        public ExitStore(NPC npc, NavMeshAgent navMeshAgent)
        {
            this.npc = npc;
            this.navMeshAgent = navMeshAgent;
        }

        public virtual void Tick()
        {
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < npc.npcSize / 2)
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
            PoolManager.Instance.ReturnToPool(npc.gameObject);
        }
    }
}