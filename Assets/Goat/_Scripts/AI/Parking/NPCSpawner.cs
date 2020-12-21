using Goat.Pooling;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.AI.Parking
{
    public class NPCSpawner : MonoBehaviour
    {
        private NPCShip ship;
        [SerializeField] private GameObject npcPrefab;
        [SerializeField] private int spawnDelay;
        protected WaitForSeconds spawnDelaySeconds;
        private GameObject currentPrefab;

        private void Awake()
        {
            ship = GetComponentInParent<NPCShip>();
            spawnDelaySeconds = new WaitForSeconds(spawnDelay);
        }

        public void SpawnNPC(int amountPassengers = 1, GameObject prefab = null)
        {
            currentPrefab = prefab == null ? npcPrefab : prefab;
            if (amountPassengers == 1)
            {
                SpawnOneNPC(currentPrefab);
            }
            else
            {
                StartCoroutine(SpawnMultipleNPC(amountPassengers));
            }
        }

        protected virtual IEnumerator SpawnMultipleNPC(int amountPassengers = 2)
        {
            int amountLeft = amountPassengers;
            while (amountLeft > 0)
            {
                yield return spawnDelaySeconds;
                SpawnOneNPC(currentPrefab);
                amountLeft--;
            }
        }

        protected void SpawnOneNPC(GameObject prefab)
        {
            GameObject npcObject = PoolManager.Instance.GetFromPool(prefab, transform.position, Quaternion.identity);
            SetupNPC(npcObject);
        }

        private void SetupNPC(GameObject npcObject)
        {
            NPC npc = npcObject.GetComponent<NPC>();
            npc.Ship = ship;
        }
    }
}