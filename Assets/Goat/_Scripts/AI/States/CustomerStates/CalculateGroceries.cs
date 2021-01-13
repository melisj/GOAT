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
        private List<Resource> availableResources = new List<Resource>();
        [HideInInspector] public bool calculatedGroceries = false;
        private float percentageWillingToSpend = 0;
        private int availableMoney = 0, calculatedCost = 0;
        private Resource[] resourcesInProject;
        private Dictionary<Resource, int> groceries = new Dictionary<Resource, int>();

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
            availableResources = availableResources.OrderBy(x => x.Price(true)).ToList();
            // Check list of available resources
            int minPrice = (int)availableResources[0].Price(true);

            // While cheapest item is still buyable
            while (minPrice < availableMoney - calculatedCost)
            {
                // I dont know how random works.
                Random.seed = Random.Range(0, 1000) + (int)Time.time;
                //Vet inefficient
                int randomIndex = Random.Range(0, availableResources.Count);
                if (availableResources[randomIndex].Price(true) < availableMoney)
                    AddResourceToGroceries(availableResources[randomIndex], 1);
            }

            Debug.LogFormat("Amount of groceries: {0}", groceries.Count);
            calculatedGroceries = true;
            customer.AmountGroceries = groceries.Count;
            return groceries;
        }

        private void AddResourceToGroceries(Resource resource, int amount)
        {
            Debug.Log($"Adding grocery {resource} x{amount}");
            if (groceries.ContainsKey(resource))
                groceries[resource] += amount;
            else
                groceries.Add(resource, amount);
            calculatedCost += (int)resource.Price(true);
        }

        public void OnEnter()
        {
            Debug.Log("Entered State Calculating Groceries");
            // Find
            calculatedGroceries = false;
            // Check customer money
            percentageWillingToSpend = Random.Range(0.7f, 1.0f);
            availableMoney = (int)(customer.money * percentageWillingToSpend);
            customer.ItemsToGet.SetInventory(GetGroceries());
        }

        public void OnExit()
        {
            customer.remainingMoney = customer.money - calculatedCost;
        }
    }
}