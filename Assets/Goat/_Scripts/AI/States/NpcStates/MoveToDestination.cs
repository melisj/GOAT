using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Goat.AI.States
{
    public class MoveToDestination : IState
    {
        private NPC npc;
        private NavMeshAgent navMeshAgent;

        private Vector3 lastLocation;
        public float timeStuck;

        // Need to check somewhere is target is reachable. NavMeshAgent.CalculatePath. NavMeshAgent.PathStatus.

        public MoveToDestination(NPC npc, NavMeshAgent navMeshAgent)
        {
            this.npc = npc;
            this.navMeshAgent = navMeshAgent;
        }

        public void Tick()
        {
            npc.searchingTime += Time.deltaTime;
            // Check if agent is stuck while navigating to target
            if (Vector3.Distance(npc.transform.position, lastLocation) <= 0)
                timeStuck += Time.deltaTime;

            lastLocation = npc.transform.position;
        }

        public void OnEnter()
        {
            Debug.LogFormat("Moving to destination");
            timeStuck = 0f;
            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(npc.targetDestination);
        }

        public void OnExit()
        {
            timeStuck = 0f;
            Debug.Log("Arrived at destination");
            //navMeshAgent.enabled = false;
        }
    }
}

