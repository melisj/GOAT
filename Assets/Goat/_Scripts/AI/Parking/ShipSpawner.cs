using Goat.Pooling;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.AI.Parking
{
    public class ShipSpawner : MonoBehaviour
    {
        [SerializeField] private ParkingSpots parkingHandler;
        [SerializeField] private Parking parkingInfo;

        [Button("Spawn Ship Random", ButtonSizes.Large)]
        private void SpawnShip()
        {
            if (Application.isPlaying)
            {
                ParkingSpots.ParkingSpot parkingSpot = parkingHandler.GetRandomParkingSpot();
                if (parkingSpot != null)
                {
                    StartCoroutine(SpawnSequence(parkingSpot));
                }
            }
            else
                Debug.LogWarning("You silly, you're trying to break things aren't ya?");
        }

        private Vector3 CalculateSpawnPosition(ParkingSpots.ParkingSpot parkingSpot)
        {
            return parkingSpot.position + GetRandomSpawn();
        }

        private Quaternion CalculateSpawnRotation(ParkingSpots.ParkingSpot parkingSpot, Vector3 spawnPosition)
        {
            Vector3 direction = (parkingSpot.position - spawnPosition);
            direction.y = 0;
            return Quaternion.LookRotation(direction, Vector3.up);
        }

        private Vector3 GetRandomSpawn()
        {
            return new Vector3(Random.Range(-50, 50), Random.Range(30, 50), Random.Range(-200, -100));
        }

        private void SetArrivingFlightPath(NPCShip ship, ParkingSpots.ParkingSpot parkingSpot)
        {
            ship.Spawner = this;
            ship.AddFlightPath(parkingSpot.position + new Vector3(0, 10, 0), 5);
            ship.AddFlightPath(parkingSpot.position, 0.1f);
            ship.ParkingSpot = parkingSpot;
        }

        // Spawn in the ship with a warp effect
        private IEnumerator SpawnSequence(ParkingSpots.ParkingSpot parkingSpot)
        {
            Vector3 spawnPosition = CalculateSpawnPosition(parkingSpot);
            Quaternion spawnRotation = CalculateSpawnRotation(parkingSpot, spawnPosition);

            PoolManager.Instance.GetFromPool(parkingInfo.warpEffectPrefab, spawnPosition, spawnRotation);

            yield return new WaitForSeconds(2);

            GameObject shipObject = PoolManager.Instance.GetFromPool(parkingInfo.shipPrefab, spawnPosition, spawnRotation);
            SetArrivingFlightPath(shipObject.GetComponentInChildren<NPCShip>(), parkingSpot);

            // Do ship animation
        }

        // Despawn the ship with a warp effect
        public IEnumerator DespawnSequence(NPCShip ship)
        {
            PoolManager.Instance.GetFromPool(parkingInfo.warpEffectPrefab, ship.transform.position, ship.transform.rotation);

            // Do ship animation

            yield return new WaitForSeconds(2);

            PoolManager.Instance.ReturnToPool(ship.gameObject);
        }
    }
}