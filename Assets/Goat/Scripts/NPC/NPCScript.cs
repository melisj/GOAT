using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
        else if (desireds.Count <= 0)
            currentAction = actionState.Checkout;

        if (currentAction == actionState.Pickup)
        {
            print($"{desireds.Count} items left, going to the {desireds.Peek().ToString()} now");
            target = GameObject.Find(desireds.Peek().ToString()).transform.position;
            desireds.Dequeue();
        }
        else if (currentAction == actionState.Checkout)
        {
            print("Going to the register");
            target = GameObject.Find("Register").transform.position;
        }
        else
        {
            print("Going to the exit");
            target = GameObject.Find("Entrance").transform.position;
        }
        agent.SetDestination(target);
    }

    void addDesiredItems()
    {
        amountOfItems = Random.Range(1, 4);
        print(amountOfItems);

        for (int i = 0; i < amountOfItems; i++)
        {
            int randomitem = Random.Range(0, desiredItem.GetNames(typeof(desiredItem)).Length);
            desireds.Enqueue((desiredItem)randomitem);
        }
    }

    IEnumerator IsInteracting()
    {
        
        arrivedAtTarget = true;
        yield return new WaitForSeconds(3f);
        targetDestination();
    }
}
