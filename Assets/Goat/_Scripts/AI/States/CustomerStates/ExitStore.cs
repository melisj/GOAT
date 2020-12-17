using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Goat.AI.States
{
    public class ExitStore : IState
    {
        private NPC npc;
        private NavMeshAgent navMeshAgent;
        private Animator animator;
        public bool exitedStore;
        Vector3 entrance;
        private float destinationDistance;

        public ExitStore(NPC npc, NavMeshAgent navMeshAgent, Animator animator)
        {
            this.npc = npc;
            this.navMeshAgent = navMeshAgent;
            this.animator = animator;
        }

        public void Tick()
        {
            if (Vector3.Distance(npc.transform.position, entrance) <= destinationDistance)
                exitedStore = true;
            animator.SetFloat("Move", navMeshAgent.velocity.sqrMagnitude);
        }

        public void OnEnter()
        {
            exitedStore = false;
            // Set animation
            entrance = GameObject.Find("Entrance").transform.position;
            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(entrance);
        }

        public void OnExit()
        {
            Debug.Log("Exited store");
            // Set animation
            navMeshAgent.enabled = false;
            animator.SetFloat("Move", 0);
        }
    }
}

