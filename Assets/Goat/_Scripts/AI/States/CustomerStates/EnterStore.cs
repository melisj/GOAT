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
        private UnloadLocations entrances;
        private float destinationDistance = 0.2f;

        public EnterStore(NPC npc, NavMeshAgent navMeshAgent, Animator animator, UnloadLocations entrances)
        {
            this.npc = npc;
            this.navMeshAgent = navMeshAgent;
            this.animator = animator;
            this.entrances = entrances;
        }

        public void Tick()
        {
            if (Vector3.Distance(npc.transform.position, entrance) <= destinationDistance)
            {
                enteredStore = true;
                //npc.enteredStore = this.enteredStore;
            }
            animator.SetFloat("Move", navMeshAgent.velocity.sqrMagnitude);
        }

        public void OnEnter()
        {
            enteredStore = false;
            navMeshAgent.enabled = true;
            // Set animation
            //entrance = GameObject.Find("Entrance").transform.position;
            entrance = entrances.Locations.GetNearest(npc.transform.position);

            navMeshAgent.SetDestination(entrance);
        }

        public void OnExit()
        {
            Debug.Log("Entered store");
            // Set animation
            navMeshAgent.enabled = false;
            npc.enterTime = Time.time;
            animator.SetFloat("Move", 0);
        }
    }
}