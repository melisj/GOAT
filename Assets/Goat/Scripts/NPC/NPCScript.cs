using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCScript : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Transform entrance;

    private NavMeshAgent agent;
    
    [SerializeField]
    private float interactionDistance = 0.5f;

    void Start()
    {
        agent = this.gameObject.GetComponent<NavMeshAgent>();
        agent.SetDestination(target.position);
    }

    void Update()
    {
        if (agent.remainingDistance < interactionDistance)
        {
            agent.SetDestination(entrance.position);
        }
    }
}
