using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Goat.AI.States
{
    public class MoveToTarget : IState
    {
        private NPC npc;
        private NavMeshAgent navMeshAgent;
        private Animator animator;

        private Vector3 lastLocation;
        public float timeStuck;


        public MoveToTarget(NPC npc, Vector3 target, NavMeshAgent navMeshAgent, Animator animator)
        {
            this.npc = npc;
            this.navMeshAgent = navMeshAgent;
            //this.animator = animator;
        }

        public void Tick()
        {
            // Check if agent is stuck while navigating to target
            if (Vector3.Distance(npc.transform.position, lastLocation) <= 0)
                timeStuck += Time.deltaTime;

            lastLocation = npc.transform.position;
        }

        public void OnEnter()
        {
            Debug.LogFormat("Moving to target");
            timeStuck = 0f;
            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(npc.targetDestination);
            // Animation
        }

        public void OnExit()
        {
            Debug.Log("Arrived at target");
            navMeshAgent.enabled = false;
            npc.searchingTime = Time.time - npc.enterTime;
            // Animation
        }
    }
}

