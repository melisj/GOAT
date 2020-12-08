using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI;

namespace Goat.Grid.Interactions
{
    public class CheckoutInteractable : BaseInteractable
    {
        private Queue<Vector3> queuePositions = new Queue<Vector3>();
        private Queue<Customer> customerQueue = new Queue<Customer>();

        public override object[] GetArgumentsForUI()
        {
            return new object[] { PeekCustomerFromQueue() };
        }

        public void AddCustomerToQueue(Customer customer)
        {
            if (customer != null && !customerQueue.Contains(customer))
                customerQueue.Enqueue(customer);
        }

        public Customer RemoveCustomerFromQueue()
        {
            return customerQueue.Dequeue();
        }

        public Customer PeekCustomerFromQueue()
        {
            if (customerQueue.Count == 0) return null; 
            return customerQueue.Peek();
        }

    }
}

