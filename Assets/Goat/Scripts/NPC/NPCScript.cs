using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Goat.Grid.Interactions;
using Goat.Manager;
using Goat.Storage;
using System.Linq;

public class NPCScript : MonoBehaviour
{
    private Transform pickup;
    [SerializeField]
    private Vector3 target;
    [SerializeField]
    private Transform entrance;

    private NavMeshAgent agent;

    int amountOfItems;

    bool arrivedAtTarget = false;

    [SerializeField]
    private float interactionDistance = 0.5f;

    private StorageInteractable targetStorage;
    private float groceriesCost = 0;

    enum actionState
    {
        Pickup,
        Checkout,
        Leave
    }
    enum desiredItem
    {
        Cheese,
        Plutonium,
        Iron
    }
    private actionState currentAction = actionState.Pickup;
    private Queue<desiredItem> desireds = new Queue<desiredItem>();
    private Dictionary<ResourceType, int> groceries = new Dictionary<ResourceType, int>();

    private void AddGroceries(ResourceType type, int amount)
    {
        if (groceries.ContainsKey(type))
            groceries[type] += amount;
        else
            groceries.Add(type, amount);
    }
    private void RemoveGroceries(ResourceType type, int amount)
    {
        if (groceries.ContainsKey(type))
        {
            groceries[type] -= amount;
            if (groceries[type] <= 0)
                groceries.Remove(type);
        }
    }

    void Start()
    {
        agent = this.gameObject.GetComponent<NavMeshAgent>();
        
        addDesiredItems();
        targetDestination();
    }

    void Update()
    {
        if (agent.remainingDistance < interactionDistance && !arrivedAtTarget)
        {
            StartCoroutine(IsInteracting());

        }
    }

    void targetDestination()
    {
        arrivedAtTarget = false;

        if (currentAction == actionState.Checkout)
            currentAction = actionState.Leave;
        else if (groceries.Count <= 0)
            currentAction = actionState.Checkout;

        if (currentAction == actionState.Pickup)
        {
            print($"{groceries.Count} items left, going to the {groceries.Keys.First().ToString()} now");
            
            bool foundTarget = false;
            ResourceType searchingType = groceries.Keys.First();

            for (int i = 0; i < NpcManager.Instance.StorageShelves.Count; i++)
            {
                for (int j = 0; j < NpcManager.Instance.StorageShelves[i].GetItems.Count; j++)
                {
                    if (searchingType == NpcManager.Instance.StorageShelves[i].GetItems[j].Resource.ResourceType)
                    {
                        targetStorage = NpcManager.Instance.StorageShelves[i];
                        target = targetStorage.transform.position;
                        foundTarget = true;
                        break;
                    }
                }
                if (foundTarget) break;
            }

            if (!foundTarget)
            {
                RemoveGroceries(searchingType, groceries[searchingType]);
                targetDestination();
            }
        }
        else if (currentAction == actionState.Checkout)
        {
            print("Going to the register");
            target = GameObject.FindGameObjectWithTag("Checkout").transform.position;
        }
        else
        {
            print("Going to the exit");
            target = GameObject.Find("Entrance").transform.position;
        }
        agent.SetDestination(target);

        

        // Check first grocery for target.
        // If no match remove item from groceries.
        // If match set target.
        // heeft hij groceries gepakt, zo niet leave weer, zo ja ga naar kassa
    }

    void addDesiredItems()
    {
        amountOfItems = Random.Range(1, 4);
        print(amountOfItems);

        //hier maken we boodschappen lijstje
        List<ResourceType> availableTypes = new List<ResourceType>(NpcManager.Instance.AvailableResources.Keys);


        for (int i = 0; i < amountOfItems; i++)
        {
            Random.seed = (int)Time.time + i;
            ResourceType typeToAdd = availableTypes[Random.Range(0, availableTypes.Count - 1)];
            AddGroceries(typeToAdd, 1);
        }
    }

    IEnumerator IsInteracting()
    {     
        //hier arriveert de NPC bij de target positie, na de timer doen we een actie (Grab, Pay, etc)
        arrivedAtTarget = true;
        yield return new WaitForSeconds(3f);
        if (currentAction == actionState.Pickup)
        {

            bool continueSearch = true;
            while (continueSearch && groceries.Count > 0)
            {
                bool foundProduct = false;
                // If target still has item grab item.
                for (int i = 0; i < targetStorage.GetItems.Count; i++)
                {
                    if (targetStorage.GetItems[i].Resource.ResourceType == groceries.Keys.First())
                    {
                        groceriesCost += targetStorage.GetItems[i].Resource.Price; // kosten die de npc moet betalen
                        targetStorage.GetResource(i, false);
                        RemoveGroceries(groceries.Keys.First(), 1);
                        foundProduct = true;
                        break;
                    }
                }
                if (!foundProduct) continueSearch = false;
            }
            // Repeat until item is no longer needed or until target is empty.
            // Look for new target.

            // Grab items from shelf
            //targetStorage.GetResource(0, false);
        } else if (currentAction == actionState.Checkout)
        {
            NpcManager.Instance.money += groceriesCost;
            //NPCManager.Instance.reputation += :) * groceriesamount;
            //paymoney
        }
        else if(currentAction == actionState.Leave)
        {
            Destroy(this.gameObject);
        }
        targetDestination();
    }
}
