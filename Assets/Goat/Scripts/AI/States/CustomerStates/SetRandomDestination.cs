using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Storage;
using Goat.Grid.Interactions;
using UnityEngine.AI;

namespace Goat.AI.States
{
    public class SetRandomDestination : IState
    {
        NPC npc;
        LayerMask layerMask;


        public SetRandomDestination(NPC npc)
        {
            this.npc = npc;
            layerMask = LayerMask.GetMask("Interactable");
        }

        public void Tick()
        {
            StorageInteractable targetStorage = GetTargetInteractable(DetectOverlap());
            if (targetStorage != null)
            {
                npc.targetStorage = targetStorage;
                npc.targetDestination = targetStorage.transform.position;
                Debug.Log("Found Storage Target");
            }
            else
            {
                npc.targetStorage = null;
                if (RandomWanderTarget(npc.transform.position, npc.wanderRange, out Vector3 wanderDestination))
                    npc.targetDestination = wanderDestination;
            }
        }

        private StorageInteractable GetTargetInteractable(Collider[] colliders)
        {
            StorageInteractable interactable = null;

            // For all interactables collided with. Check if any resources is located inside.
            for (int c = 0; c < colliders.Length; c++)
            {
                // Check all resources inside interactable
                StorageInteractable tempInteractable = colliders[c].GetComponent<StorageInteractable>();
                if (tempInteractable != null)
                {
                    for (int i = 0; i < tempInteractable.GetItemCount; i++)
                    {
                        // Check if any resource inside interactable is located inside groceries dictionary
                        if (npc.itemsToGet.ContainsKey(tempInteractable.GetItems[i].Resource))
                        {
                            // Now takes the first match, might change to match that is located closest to customer
                            return tempInteractable;
                        }
                    }
                }
            }

            return interactable;
        }

        private Collider[] DetectOverlap()
        {
            return Physics.OverlapSphere(npc.transform.position, npc.npcSize, layerMask);
        }


        /// <summary>
        /// Get random position on NavMesh
        /// </summary>
        /// <param name="center"> Origin of wander range</param>
        /// <param name="range"> Range from origin to wander inside</param>
        /// <param name="result"> Random position on NavMesh</param>
        /// <returns></returns>
        protected bool RandomWanderTarget(Vector3 center, float range, out Vector3 result)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomPoint = center + Random.insideUnitSphere * range;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
                { 
                    result = hit.position;
                    Debug.LogFormat("Hit NavMesh on: {0}", result);
                    return true;
                }
            }
            Debug.LogWarning("No hit on NavMesh!");
            result = Vector3.zero;
            return false;
        }

        public void OnEnter()
        {
            Debug.Log("Searching for target");
            
        }

        public void OnExit()
        {
            npc.searchingTime = Time.time - npc.enterTime;
        }
    }
}

