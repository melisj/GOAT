using Goat.Pooling;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Goat.AI.Parking
{
    [System.Serializable]
    public class FlightPath
    {
        public Vector3 RelativePosition;
        public float accuracy;
    }

    public class ShipSpawner : MonoBehaviour
    {
        [SerializeField] private ParkingSpots parkingHandler;
        [SerializeField] private Parking parkingInfo;

        [Header("Paths")]
        [SerializeField] private FlightPath[] arrivalPath;
        [SerializeField] private FlightPath[] departurePath;

        public FlightPath[] ArrivalPath => arrivalPath;
        public FlightPath[] DeparturePath => departurePath;

        [Header("Debug")]
        [SerializeField] private bool debug;
        [SerializeField, ShowIf("debug")] private Color lineColor;
        [SerializeField, ShowIf("debug")] private Color sphereColor;

        [Button("Spawn Ship Random", ButtonSizes.Large)]
        public virtual void SpawnShip(int amountPassengers)
        {
            if (Application.isPlaying)
            {
                ParkingSpots.ParkingSpot parkingSpot = parkingHandler.GetRandomParkingSpot();
                if (parkingSpot != null)
                {
                    StartCoroutine(SpawnSequence(parkingSpot, amountPassengers));
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

        protected virtual void SetArrivingFlightPath(NPCShip ship, ParkingSpots.ParkingSpot parkingSpot, int amountPassengers)
        {
            ship.AmountPassengers = amountPassengers;
            ship.Spawner = this;
            SetFlightPath(ship, arrivalPath, parkingSpot);
            ship.ParkingSpot = parkingSpot;
        }

        public virtual void SetFlightPath(NPCShip ship, FlightPath[] flightPath, ParkingSpots.ParkingSpot parkingSpot)
        {
            foreach(FlightPath path in flightPath)
            {
                ship.AddFlightPath(path.RelativePosition + parkingSpot.position, path.accuracy);
            }
        }

        protected virtual IEnumerator SpawnSequence(ParkingSpots.ParkingSpot parkingSpot, int amountPassengers)
        {
            Vector3 spawnPosition = CalculateSpawnPosition(parkingSpot);
            Quaternion spawnRotation = CalculateSpawnRotation(parkingSpot, spawnPosition);

            PoolManager.Instance.GetFromPool(parkingInfo.warpEffectPrefab, spawnPosition, spawnRotation);

            yield return new WaitForSeconds(2);

            GameObject shipObject = PoolManager.Instance.GetFromPool(parkingInfo.shipPrefab, spawnPosition, spawnRotation);
            SetArrivingFlightPath(shipObject.GetComponentInChildren<NPCShip>(), parkingSpot, amountPassengers);

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

        private void OnDrawGizmos()
        {
            if (debug)
            {
                if (parkingHandler.ParkingList != null && parkingHandler.ParkingList.Length != 0)
                {
                    Vector3 firstSpot = parkingHandler.ParkingList[0].position;
                    ShowPath(firstSpot, arrivalPath);
                    ShowPath(firstSpot, departurePath);
                }
            }
        }

        private void ShowPath(Vector3 fromPosition, FlightPath[] flightPath)
        {
            for (int i = 0; i < flightPath.Length; i++)
            {
                Gizmos.color = sphereColor;
                Gizmos.DrawSphere(fromPosition + flightPath[i].RelativePosition, 1f);

                Gizmos.color = lineColor;
                if (i == 0) 
                    Gizmos.DrawLine(fromPosition, fromPosition + flightPath[i].RelativePosition);
                else if(i == flightPath.Length - 2)
                    Gizmos.DrawLine(fromPosition + flightPath[i].RelativePosition, fromPosition + flightPath[i + 1].RelativePosition);

                Gizmos.color = sphereColor - new Color(0, 0, 0, 0.6f);
                Gizmos.DrawSphere(fromPosition + flightPath[i].RelativePosition, flightPath[i].accuracy);
            }
        }
    }
}