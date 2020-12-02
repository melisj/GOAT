using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Goat.Grid.Interactions;
using Goat.Manager;
using Goat.Storage;
using System.Linq;
using Goat.Pooling;

public class NPCScript : MonoBehaviour, IPoolObject
{
    private Transform pickup;
    [SerializeField]
    private Vector3 target;
    [SerializeField]
    private Transform entrance;
    private Money money;

    private NavMeshAgent agent;

    private int amountOfItems;

    private bool arrivedAtTarget = false;

    [SerializeField]
    private float interactionDistance = 0.5f;

    private StorageInteractable targetStorage;
    private float groceriesCost = 0;
    public int PoolKey { get; set; }
    public ObjectInstance ObjInstance { get; set; }

    private enum actionState
    {
        Pickup,
        Checkout,
        Leave
    }

    private enum desiredItem
    {
        Cheese,
        Plutonium,
        Iron
    }

    private actionState currentAction = actionState.Pickup;
    private Queue<desiredItem> desireds = new Queue<desiredItem>();
    private Dictionary<ResourceType, int> groceries = new Dictionary<ResourceType, int>();
    private Dictionary<Resource, int> groceriesResource = new Dictionary<Resource, int>();

    public void OnGetObject(ObjectInstance objectInstance, int poolKey)
    {
        ObjInstance = objectInstance;
        PoolKey = poolKey;
    }

    public void OnReturnObject()
    {
        StopAllCoroutines();
        agent.enabled = false;
        gameObject.SetActive(false);
    }

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

    private void Start()
    {
        //  agent = this.gameObject.GetComponent<NavMeshAgent>();
    }

    public void Setup()
    {
        agent = this.gameObject.GetComponent<NavMeshAgent>();
        currentAction = actionState.Pickup;
        addDesiredItems();
        agent.enabled = true;
        targetDestination();
    }

    private void Update()
    {
        if (agent.isActiveAndEnabled && agent.isOnNavMesh && agent.remainingDistance < interactionDistance && !arrivedAtTarget)
        {
            StartCoroutine(IsInteracting());
        }
    }

    private void targetDestination()
    {
        arrivedAtTarget = false;

        if (currentAction == actionState.Checkout)
            currentAction = actionState.Leave;
        else if (groceries.Count <= 0)
            currentAction = actionState.Checkout;

        if (currentAction == actionState.Pickup)
        {
            //  print($"{groceries.Count} items left, going to the {groceries.Keys.First().ToString()} now");

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
            //   print("Going to the register");
            target = GameObject.FindGameObjectWithTag("Checkout").transform.position;
        }
        else
        {
            //   print("Going to the exit");
            target = GameObject.Find("Entrance").transform.position;
        }
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.SetDestination(target);
        }

        // Check first grocery for target.
        // If no match remove item from groceries.
        // If match set target.
        // heeft hij groceries gepakt, zo niet leave weer, zo ja ga naar kassa
    }

    private void addDesiredItems()
    {
        amountOfItems = Random.Range(1, 4);
        //print(amountOfItems);

        //hier maken we boodschappen lijstje
        List<ResourceType> availableTypes = new List<ResourceType>(NpcManager.Instance.AvailableResources.Keys);

        for (int i = 0; i < amountOfItems; i++)
        {
            Random.seed = (int)Time.time + i;
            ResourceType typeToAdd = availableTypes[Random.Range(0, availableTypes.Count - 1)];
            AddGroceries(typeToAdd, 1);
        }
    }

    private IEnumerator IsInteracting()
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
                    if (targetStorage.GetItems[i].Resource == groceriesResource.Keys.First())
                    {
                    }
                    if (targetStorage.GetItems[i].Resource.ResourceType == groceries.Keys.First())
                    {
                        if (money == null)
                            money = targetStorage.GetItems[i].Resource.Money;

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
        }
        else if (currentAction == actionState.Checkout)
        {
            if (money != null)
            {
                money.Amount += groceriesCost;
            }
            //NPCManager.Instance.reputation += :) * groceriesamount;
            //paymoney
        }
        else if (currentAction == actionState.Leave)
        {
            //Destroy(this.gameObject);
            PoolManager.Instance.ReturnToPool(gameObject);
        }
        targetDestination();
    }
}