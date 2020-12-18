using Goat.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.AI.Parking
{
    public class NPCSpawner : MonoBehaviour
    {
        private NPCShip ship;
        [SerializeField] private GameObject npcPrefab;

        public void Awake()
        {
            ship = GetComponentInParent<NPCShip>();
        }

        public void SpawnNPC()
        {
            GameObject customerObject = PoolManager.Instance.GetFromPool(npcPrefab, transform.position, Quaternion.identity);
            Customer customer = customerObject.GetComponent<Customer>();

            customer.Ship = ship;
        }
    }
}