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
        protected Animator animator;
        public bool exitedStore;
        protected Vector3 entrance;
        protected float destinationDistance;

        public ExitStore(NPC npc, NavMeshAgent navMeshAgent, Animator animator)
        {
            this.npc = npc;
            this.navMeshAgent = navMeshAgent;
            this.animator = animator;
        }

        public virtual void Tick()
        {
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < npc.npcSize / 2)
            {
                OnExit();
            }
            animator.SetFloat("Move", navMeshAgent.velocity.sqrMagnitude);
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
            animator.SetFloat("Move", 0);
            npc.Ship.ShipReadyToFly();
            PoolManager.Instance.ReturnToPool(npc.gameObject);
        }
    }
}