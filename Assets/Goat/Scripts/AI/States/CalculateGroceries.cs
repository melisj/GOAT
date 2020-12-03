using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Storage;
using UnityEngine.AI;

namespace Goat.AI.States
{
    public class CalculateGroceries : IState
    {
        private Customer customer;
        private List<Resource> availableResources;
        [HideInInspector] public bool calculatedGroceries = false;
        private float percentageWillingToSpend = 0;
        private int availableMoney = 0, calculatedCost = 0;
        public CalculateGroceries(Customer customer)
        {
            this.customer = customer;
        }
        public void Tick()
        {
            customer.groceries = GetGroceries();
        }

        private Dictionary<Resource, int> GetGroceries()
        {
            Dictionary<Resource, int> groceries = new Dictionary<Resource, int>();
            // Check customer money or grocerycap
            
            // Build list of available resources

            // Sort list of avaiable resources on price;

            // Check list of available resources

            calculatedGroceries = true;
            return groceries;
        }

        public void OnEnter()
        {
            // Find
            calculatedGroceries = false;
            Random.seed = (int)Time.time;
            percentageWillingToSpend = Random.Range(7.0f, 10.0f);
            availableMoney = (int)(customer.money * percentageWillingToSpend);
        }
        public void OnExit()
        {
            customer.remainingMoney = customer.money - calculatedCost;
        }
    }
}

