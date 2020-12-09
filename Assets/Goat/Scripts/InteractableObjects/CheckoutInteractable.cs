using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI;

namespace Goat.Grid.Interactions
{
    public class CheckoutInteractable : MonoBehaviour
    {
        private Queue<Vector3> queuePositions = new Queue<Vector3>();
        private Queue<Customer> customerQueue = new Queue<Customer>();


        public void AddCustomerToQueue(Customer customer)
        {
            if (customer != null && !customerQueue.Contains(customer))
                customerQueue.Enqueue(customer);
        }
        public void RemoveCustomerFromQueue(Customer customer)
        {
            if (customer != null && customerQueue.Contains(customer))
                customerQueue.Dequeue();
        }

    }
}

