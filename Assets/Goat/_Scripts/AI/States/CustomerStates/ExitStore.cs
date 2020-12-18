using Goat.AI.Satisfaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Goat.Pooling;

namespace Goat.AI.States
{
    public class ExitStore : IState
    {
        private NPC npc;
        private CustomerReview review;
        private NavMeshAgent navMeshAgent;
        private Animator animator;
        public bool exitedStore;
        private Vector3 entrance;
        private float destinationDistance;
        private UnloadLocations entrances;

        public ExitStore(NPC npc, NavMeshAgent navMeshAgent, Animator animator, CustomerReview review, UnloadLocations entrances)
        {
            this.npc = npc;
            this.navMeshAgent = navMeshAgent;
            this.animator = animator;
            this.review = review;
            this.entrances = entrances;
        }

        public void Tick()
        {
            if (navMeshAgent.remainingDistance < npc.npcSize / 2)
            {
                OnExit();
            }
            //if (Vector3.Distance(npc.transform.position, entrance) <= destinationDistance)
            //    exitedStore = true;
            animator.SetFloat("Move", navMeshAgent.velocity.sqrMagnitude);
        }

        public void OnEnter()
        {
            exitedStore = false;
            Debug.Log("Exited store STATE");

            // Set animation
            //entrance = GameObject.Find("Entrance").transform.position;
            entrance = entrances.Locations.GetNearest(npc.transform.position);
            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(entrance);
        }

        public void OnExit()
        {
            Debug.Log("Exited store");
            review.WriteReview();
            // Set animation
            navMeshAgent.enabled = false;
            animator.SetFloat("Move", 0);
            PoolManager.Instance.ReturnToPool(npc.gameObject);
        }
    }
}