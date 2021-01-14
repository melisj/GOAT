using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkingAnimation : MonoBehaviour
{

    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator animator;
    [Range(0.0f,1.0f)]
    [SerializeField] private float threshold = 0.1f;

    // Update is called once per frame
    void Update()
    {
        float agentSpeed = navMeshAgent.velocity.normalized.magnitude > threshold ? 1 : 0;

        animator?.SetFloat("Move", agentSpeed);
    }
}
