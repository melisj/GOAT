using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Goat.AI.States
{
    public class EnterStore : IState
    {
        protected NPC npc;
        protected NavMeshAgent navMeshAgent;
        protected Animator animator;
        public bool enteredStore;
        protected Vector3 entrance;
        protected UnloadLocations entrances;
        protected float destinationDistance = 0.2f;

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

        public virtual void OnEnter()
        {
            enteredStore = false;
            navMeshAgent.enabled = true;
            // Set animation
            //entrance = GameObject.Find("Entrance").transform.position;
            entrance = entrances.Locations.GetNearest(npc.transform.position);
            navMeshAgent.SetDestination(entrance);
        }

        public virtual void OnExit()
        {
            // Set animation
            //navMeshAgent.enabled = false;
            npc.enterTime = Time.time;
            animator.SetFloat("Move", 0);
        }
    }
}