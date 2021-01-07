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
        CheckoutInteractable[] checkoutArray;
        public SearchForCheckout(Customer customer)
        {
            this.customer = customer;
        }

        private Vector3 FindCheckout()
        {   
            // Cleaner with scriptableobject that tracks checkouts?
            checkoutArray = Object.FindObjectsOfType<CheckoutInteractable>().Where(x => x.QueueAvailable).OrderBy(x => x.QueueLength).ToArray();
            if (checkoutArray.Length == 0)
                return Vector3.zero;
            
            CheckoutInteractable tempCheckout = checkoutArray.First();

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
            //Debug.LogFormat("Customer has {0} items left to get. Customer has been searching for {1} / {2} seconds. Searching for checkout = {3}", customer.itemsToGet.Count, customer.searchingTime, customer.maxSearchingTime, checks);
            if (checks == 0)
                Debug.Log("Searching for checkout");
            else if (checks == 1)
                Debug.Log("Searching for shortest queue");
            checks++;
            Vector3 checkoutPosition = FindCheckout();
            if (checkoutPosition == Vector3.zero) checks = 0;
            customer.targetDestination = checkoutPosition;
            customer.leavingStore = true;
        }

        public void OnExit()
        {

        }
    }
}

