using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Storage;
using Goat.Grid.Interactions;
using UnityEngine.AI;

namespace Goat.AI.States
{
    /// <summary>
    /// Set random destination for AI to move to
    /// </summary>
    public class SetRandomDestination : IState
    {
        private const int chanceToSpeak = 50;

        private NPC npc;
        private NavMeshAgent navMeshAgent;
        private LayerMask layerMask;
        private StorageInteractable prevStorage;
        private Vector3 wanderDestination;
        private NavMeshPath navPath = new NavMeshPath();
        private int areaMask;
        private AudioCue audio;

        public SetRandomDestination(NPC npc, NavMeshAgent navMeshAgent, int areaMask, AudioCue audio = null)
        {
            this.npc = npc;
            this.navMeshAgent = navMeshAgent;
            layerMask = LayerMask.GetMask("Interactable");
            this.areaMask = areaMask;
            this.audio = audio;
        }

        public void Tick()
        {
            npc.SearchingTime += Time.deltaTime;

            CalculateDestination();
        }

        /// <summary>
        /// Try to get a new destination for the AI to move to
        /// </summary>
        private void CalculateDestination()
        {
            Vector3 tempDestination;
            bool hitDestination = false;
            if (RandomStorageTarget(DetectOverlap(), out tempDestination))
            {
                wanderDestination = tempDestination;
                hitDestination = true;
            }
            else if (RandomWanderTarget(npc.transform.position, range: npc.WanderRange, out tempDestination))
            {
                int random = Random.Range(0, 101);
                if (random > chanceToSpeak && audio != null)
                    audio.PlayAudioCue();
                wanderDestination = tempDestination;
                hitDestination = true;
            }
            //else continue;

            // If a destination was hit check if the destination is reachable
            if (hitDestination)
            {
                NavMeshQueryFilter filter = new NavMeshQueryFilter
                {
                    areaMask = (1 << areaMask)
                };
                if (NavMesh.CalculatePath(navMeshAgent.transform.position, wanderDestination, filter, navPath))
                {
                    Debug.LogWarning("Invalid path calculated");
                    //continue;
                }
                else
                {
                    npc.TargetDestination = wanderDestination;
                    Debug.Log("Found an new destination!");
                }
            }
        }

        private Collider[] DetectOverlap()
        {
            return Physics.OverlapSphere(npc.transform.position, radius: 4, layerMask);
        }

        /// <summary>
        /// Tries to move to a random storage shelve inside the store
        /// </summary>
        /// <param name="colliders"> Correct colliders hit </param>
        /// <param name="result"> Vector3 destination to move to </param>
        /// <returns></returns>
        private bool RandomStorageTarget(Collider[] colliders, out Vector3 result)
        {
            result = npc.transform.position;
            if (colliders.Length < 1) return false;

            int randomIndex = Random.Range(0, colliders.Length);

            StorageInteractable tempInteractable = prevStorage;

            if (colliders[randomIndex].tag == "Storage")
            {
                tempInteractable = colliders[randomIndex].GetComponentInParent<StorageInteractable>();
                if (prevStorage != tempInteractable)
                {
                    // The location to wander to + a random offset
                    Vector3 tempResult = tempInteractable.transform.position + ((tempInteractable.transform.forward * Random.Range(0, 1f)) + (tempInteractable.transform.right * Random.Range(-.5f, .5f)));
                    result = tempResult;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Get random position on NavMesh
        /// </summary>
        /// <param name="center"> Origin of wander range</param>
        /// <param name="range"> Range from origin to wander</param>
        /// <param name="result"> Random position on NavMesh</param>
        /// <returns></returns>
        private bool RandomWanderTarget(Vector3 center, float range, out Vector3 result)
        {
            Vector3 randomPoint = center + (Random.insideUnitSphere * range);
            NavMeshHit hit;
            NavMeshQueryFilter filter = new NavMeshQueryFilter
            {
                areaMask = (1 << areaMask)
            };
            while (!NavMesh.SamplePosition(randomPoint, out hit, range * 5, filter))
            {
                result = hit.position;
                Debug.LogFormat("Hit NavMesh on: {0}", result);
                return true;
            }

            //Debug.LogWarning("No hit on NavMesh!");
            result = npc.transform.position;
            return false;
        }

        public void OnEnter()
        {
            Debug.Log("Searching for target");
            npc.TargetStorage = null;
            npc.TargetDestination = npc.transform.position;
            wanderDestination = npc.transform.position;
            navMeshAgent.enabled = true;
        }

        public void OnExit()
        {
            //navMeshAgent.enabled = false;
        }
    }
}