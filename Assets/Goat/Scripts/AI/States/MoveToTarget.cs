using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Goat.AI.States
{
    public class MoveToTarget : IState
    {
        private Transform agent;
        private Vector3 target;
        private NavMeshAgent navMeshAgent;
        private Animator animator;

        private Vector3 lastLocation;
        public float timeStuck;


        public MoveToTarget(Transform agent, Vector3 target, NavMeshAgent navMeshAgent, Animator animator)
        {
            this.agent = agent;
            this.target = target;
            this.navMeshAgent = navMeshAgent;
            //this.animator = animator;
        }

        public void Tick()
        {
            // Check if agent is stuck while navigating to target
            if (Vector3.Distance(agent.position, lastLocation) <= 0)
                timeStuck += Time.deltaTime;

            lastLocation = agent.position;
        }

        public void OnEnter()
        {
            timeStuck = 0f;
            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(target);
            // Animation
        }

        public void OnExit()
        {
            navMeshAgent.enabled = false;
            // Animation
        }
    }
}

