using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Goat.AI.States
{
    /// <summary>
    /// EnterStore state for warehouse worker so it moves to the storage area immideatly;
    /// </summary>
    public class EnterGoToStorage : IState
    {
        public bool entered = false;
        private Worker worker;
        private NavMeshAgent navAgent;
        private Animator animator;
        private Vector3 target;

        public EnterGoToStorage(Worker worker, NavMeshAgent navAgent, Animator animator)
        {
            this.worker = worker;
            this.navAgent = navAgent;
            this.animator = animator;
        }

        public void Tick()
        {
            if (navAgent.remainingDistance < worker.npcSize / 2)
                entered = true;

            animator.SetFloat("Move", navAgent.velocity.sqrMagnitude);
        }

        //fuck this fix.
        private void FindStorageThing()
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag("Container");

            if(targets != null && targets.Length > 0)
            {
                target = targets[Random.Range(0, targets.Length)].transform.position;
            }
            else
            {
                target = GameObject.FindGameObjectWithTag("Entrance").transform.position;
            }
        }

        public void OnEnter()
        {
            navAgent.enabled = true;
            FindStorageThing();
            navAgent.SetDestination(target);
        }

        public void OnExit()
        {
        }
    }
}

