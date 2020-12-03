using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Storage;
using UnityEngine.AI;
using System.Linq;

namespace Goat.AI.States
{
    public class CalculateGroceries : IState
    {
        private Customer customer;
        private List<Resource> availableResources;
        [HideInInspector] public bool calculatedGroceries = false;
        private float percentageWillingToSpend = 0;
        private int availableMoney = 0, calculatedCost = 0;
        Resource[] resourcesInProject;
        Dictionary<Resource, int> groceries = new Dictionary<Resource, int>();

        public CalculateGroceries(Customer customer, Resource[] resourcesInProject)
        {
            this.customer = customer;
            this.resourcesInProject = resourcesInProject;
        }
        public void Tick()
        {

        }

        private Dictionary<Resource, int> GetGroceries()
        {
            // Build list of available resources
            for (int i = 0; i < resourcesInProject.Length; i++)
                if (resourcesInProject[i].Available) availableResources.Add(resourcesInProject[i]);
            // Sort list of avaiable resources on price;
            availableResources = availableResources.OrderBy(x => x.Price).ToList();
            // Check list of available resources
            int minPrice = (int)availableResources[0].Price;

            // While cheapest item is still buyable
            while (availableMoney - calculatedCost > minPrice)
            {
                //Vet inefficient
                int randomIndex = (int)Random.Range(0, availableResources.Count - 0.1f);
                if (availableResources[randomIndex].Price < availableMoney)
                    AddResourceToGroceries(availableResources[randomIndex], 1);
            }

            calculatedGroceries = true;
            return groceries;
        }

        private void AddResourceToGroceries(Resource resource, int amount)
        {
            if (groceries.ContainsKey(resource))
                groceries[resource] += amount;
            else
                groceries.Add(resource, amount);
        }

        public void OnEnter()
        {
            // Find
            calculatedGroceries = false;
            // Check customer money
            Random.seed = (int)Time.time;
            percentageWillingToSpend = Random.Range(7.0f, 10.0f);
            availableMoney = (int)(customer.money * percentageWillingToSpend);
            customer.itemsToGet = GetGroceries();
        }
        public void OnExit()
        {
            customer.remainingMoney = customer.money - calculatedCost;
        }
    }
}

