using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Goat.Grid.Interactions;
using System.Linq;

namespace Goat.AI.States
{
    public class SearchForCheckout : IState
    {
        private Customer customer;
        public int checks = 0;
        public bool inQueue = false;

        public SearchForCheckout(Customer customer)
        {
            this.customer = customer;
        }

        private Vector3 FindCheckout()
        {   
            // Cleaner with scriptableobject that tracks checkouts?
            CheckoutInteractable[] checkouts = Object.FindObjectsOfType<CheckoutInteractable>().Where(x => x.QueueAvailable).OrderBy(x => x.QueueLength).ToArray();
            CheckoutInteractable tempCheckout = checkouts.First();

            if (tempCheckout != null)
            {
                Vector3 queuePosition = tempCheckout.LastPositionInQueue;
                if (checks == 2)
                {
                    tempCheckout.AddCustomerToQueue(customer);
                    inQueue = true;
                }
                return queuePosition;
            }
            else
                return Vector3.zero;
        }

        public void Tick()
        {
        }

        public void OnEnter()
        {
            if (checks == 0)
                Debug.Log("Searching for checkout");
            else if (checks == 1)
                Debug.Log("Searching for shortest queue");
            checks++;
            customer.targetDestination = FindCheckout();
        }

        public void OnExit()
        {

        }
    }
}

