﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

namespace Goat.AI.States
{
    /// <summary>
    /// Move to the location of the targed.
    /// </summary>
    [System.Serializable]
    public class MoveToTarget : IState
    {
        private NPC npc;
        private NavMeshAgent navMeshAgent;
        [SerializeField, ReadOnly] private Vector3 lastLocation;
        [SerializeField, ReadOnly] private float timeStuck;
        [SerializeField, ReadOnly] private int amountStuckCalled;
        public float TimeStuck => timeStuck;
        public int AmountStuckCalled => amountStuckCalled;

        public MoveToTarget(NPC npc, NavMeshAgent navMeshAgent)
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
            Debug.LogFormat("Moving to target");
            timeStuck = 0f;
            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(npc.TargetStorage.transform.position);
        }

        public void OnExit()
        {
            timeStuck = 0f;
            Debug.Log("Arrived at target");
            //navMeshAgent.enabled = false;
            navMeshAgent.ResetPath();
        }
    }
}