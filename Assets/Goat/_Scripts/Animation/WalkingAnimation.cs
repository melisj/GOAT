using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkingAnimation : MonoBehaviour
{

    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator animator;

    // Update is called once per frame
    void Update()
    {
        animator?.SetFloat("Move", navMeshAgent.velocity.sqrMagnitude);
    }
}
