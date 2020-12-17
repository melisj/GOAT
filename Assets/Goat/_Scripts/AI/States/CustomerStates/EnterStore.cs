using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Goat.AI.States
{
    public class EnterStore : IState
    {
        private Customer customer;
        private NavMeshAgent navMeshAgent;
        private Animator animator;
        public bool enteredStore;
        private Vector3 entrance;
        private float destinationDistance = 3;
        private UnloadLocations entrances;

        public EnterStore(Customer customer, NavMeshAgent navMeshAgent, Animator animator, UnloadLocations entrances)
        {
            this.customer = customer;
            this.navMeshAgent = navMeshAgent;
            this.animator = animator;
            this.entrances = entrances;
        }

        public void Tick()
        {
            if (Vector3.Distance(customer.transform.position, entrance) <= destinationDistance)
            {
                enteredStore = true;
                customer.enteredStore = this.enteredStore;
            }
        }

        public void OnEnter()
        {
            enteredStore = false;
            navMeshAgent.enabled = true;
            // Set animation
            //entrance = GameObject.Find("Entrance").transform.position;
            entrance = entrances.Locations.GetNearest(customer.transform.position);

            navMeshAgent.SetDestination(entrance);
        }

        public void OnExit()
        {
            Debug.Log("Entered store");
            // Set animation
            navMeshAgent.enabled = false;
            customer.enterTime = Time.time;
        }
    }
}