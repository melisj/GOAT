using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Goat.AI
{
    public class RotateAgent : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 10;
        NavMeshAgent agent;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void OnEnable()
        {
            agent.updateRotation = false;
        }

        private void OnDisable()
        {
            agent.updateRotation = true;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (agent.velocity.sqrMagnitude > 0)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(agent.velocity.normalized), rotationSpeed * Time.deltaTime);
        }
    }
}

