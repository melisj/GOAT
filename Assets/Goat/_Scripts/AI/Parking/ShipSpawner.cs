using Goat.Pooling;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.AI.Parking
{
    public class ShipSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject shipPrefab;
        [SerializeField] private GameObject spawnPrefab;
        [SerializeField] private ParkingSpots parkingHandler;
        [SerializeField] private Parking parkingInfo;

        [Button("Spawn Ship Random", ButtonSizes.Large)]
        private void SpawnShip()
        {
            parkingInfo.CurrentSpotSpawing = parkingHandler.GetRandomParkingSpot();
            if (parkingInfo.CurrentSpotSpawing != null)
            {
                StartCoroutine(SpawnSequence());
            }
        }

        private Vector3 CalculateSpawnPosition()
        {
            return parkingInfo.CurrentSpotSpawing.position + GetRandomSpawn();
        }

        private Quaternion CalculateSpawnRotation(Vector3 spawnPosition)
        {
            Vector3 direction = (parkingInfo.CurrentSpotSpawing.position - spawnPosition);
            direction.y = 0;
            return Quaternion.LookRotation(direction, Vector3.up);
        }

        private Vector3 GetRandomSpawn()
        {
            return new Vector3(Random.Range(-20, 20), Random.Range(10, 30), Random.Range(-200, 0));
        }

        private IEnumerator SpawnSequence()
        {
            Vector3 SpawnPosition = CalculateSpawnPosition();
            Quaternion SpawnRotation = CalculateSpawnRotation(SpawnPosition);

            PoolManager.Instance.GetFromPool(spawnPrefab, SpawnPosition, SpawnRotation);

            yield return new WaitForSeconds(2);
            
            PoolManager.Instance.GetFromPool(shipPrefab, SpawnPosition, SpawnRotation);
        }
    }
}