using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Goat.AI.States
{
    public class EnterStore : IState
    {
        private NPC npc;
        private NavMeshAgent navMeshAgent;
        private Animator animator;
        public bool enteredStore;
        Vector3 entrance;
        private float destinationDistance = 3;

        public EnterStore(NPC npc, NavMeshAgent navMeshAgent, Animator animator)
        {
            this.npc = npc;
            this.navMeshAgent = navMeshAgent;
            this.animator = animator;
        }

        public void Tick()
        {
            if (Vector3.Distance(npc.transform.position, entrance) <= destinationDistance)
                enteredStore = true;
        }

        public void OnEnter()
        {
            enteredStore = false;
            // Set animation
            entrance = GameObject.Find("Entrance").transform.position;
            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(entrance);
        }

        public void OnExit()
        {
            Debug.Log("Entered store");
            // Set animation
            navMeshAgent.enabled = false;
            npc.enterTime = Time.time;
        }
    }
}

