using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

namespace Goat.AI.States
{
    /// <summary>
    /// Make nevmeshagent move to destination selected by NPC
    /// </summary>
    [System.Serializable]
    public class MoveToDestination : IState
    {
        private NPC npc;
        private NavMeshAgent navMeshAgent;
        [SerializeField, ReadOnly] private Vector3 lastLocation;
        [SerializeField, ReadOnly] private float timeStuck;
        [SerializeField, ReadOnly] private int amountStuckCalled;

        // Need to check somewhere is target is reachable. NavMeshAgent.CalculatePath. NavMeshAgent.PathStatus.
        public float TimeStuck => timeStuck;

        public int AmountStuckCalled => amountStuckCalled;

        public MoveToDestination(NPC npc, NavMeshAgent navMeshAgent)
        {
            this.npc = npc;
            this.navMeshAgent = navMeshAgent;
        }

        public void Tick()
        {
            npc.SearchingTime += Time.deltaTime;
            float dist = Vector3.Distance(npc.transform.position, lastLocation);
            // Check if agent is stuck while navigating to target
            if (dist <= 0.001f)
            {
                amountStuckCalled++;
                timeStuck += Time.deltaTime;
            }

            lastLocation = npc.transform.position;
        }

        public void OnEnter()
        {
            timeStuck = 0f;
            amountStuckCalled = 0;
            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(npc.TargetDestination);
        }

        public void OnExit()
        {
            timeStuck = 0f;
            navMeshAgent.ResetPath();
        }
    }
}