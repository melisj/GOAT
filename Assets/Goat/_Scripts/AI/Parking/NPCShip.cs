using Goat.Pooling;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.AI.Parking
{
    public class NPCShip : MonoBehaviour, IPoolObject
    {
        private ParkingSpots.ParkingSpot parkingSpot;

        public ParkingSpots.ParkingSpot ParkingSpot
        {
            get { return parkingSpot; }
            set { parkingSpot = value; LandShip(); }
        }

        public Queue<Vector3> flightPath = new Queue<Vector3>();
        public Queue<float> accuracyOnPath = new Queue<float>();

        public NPCSpawner NpcSpawner { get; set; }
        public ShipSpawner Spawner { get; set; }
        public int AmountPassengers { get; set; }
        public int PoolKey { get; set; }
        public ObjectInstance ObjInstance { get; set; }

        // Flight speed regulation
        [SerializeField] private AnimationCurve speedCurve;

        private void Awake()
        {
            NpcSpawner = GetComponentInChildren<NPCSpawner>();
        }

        public void AddFlightPath(Vector3 location, float accuracy)
        {
            flightPath.Enqueue(location);
            accuracyOnPath.Enqueue(accuracy);
        }

        protected virtual void ShipHasLanded()
        {
            // Spawn NPC
            NpcSpawner.SpawnNPC(AmountPassengers);
        }

        public virtual void ShipReadyToFly()
        {
            // Customer has arrived
            parkingSpot.ocupied = false;
            Spawner.SetFlightPath(this, Spawner.DeparturePath, ParkingSpot);
            StartCoroutine(FollowFlightPath(() => ShipHasDeparted()));
        }

        private void ShipHasDeparted()
        {
            // Despawn this
            StartCoroutine(Spawner.DespawnSequence(this));
        }

        private void LandShip()
        {
            StartCoroutine(FollowFlightPath(() => ShipHasLanded()));
        }

        private IEnumerator FollowFlightPath(Action callback)
        {
            if (flightPath.Count == 0) { Debug.LogWarning("No flight path was assigned"); yield break; }

            Vector3 startingPos = transform.position;
            float currentTime = 0;
            float totalDistance = Vector3.Distance(startingPos, flightPath.Peek());
            float currentDistance = totalDistance;

            while (flightPath.Count != 0)
            {
                // Lerp ship closer to its destination with the speed curve in mind
                currentTime += Time.deltaTime * speedCurve.Evaluate(1 - (currentDistance / totalDistance));
                transform.position = Vector3.Lerp(startingPos, flightPath.Peek(), currentTime);

                Vector3 direction = flightPath.Peek() - startingPos;
                direction.y = 0;
                transform.rotation = Quaternion.Slerp(Quaternion.LookRotation(direction, Vector3.up), transform.rotation, Time.deltaTime);

                currentDistance = Vector3.Distance(transform.position, flightPath.Peek());

                // Set new flightpath when this flight path is close enough
                if (currentDistance < accuracyOnPath.Peek())
                {
                    flightPath.Dequeue();
                    accuracyOnPath.Dequeue();
                    startingPos = transform.position;
                    currentTime = 0;

                    if (flightPath.Count != 0)
                        totalDistance = Vector3.Distance(startingPos, flightPath.Peek());
                }

                yield return null;
            }

            callback.Invoke();
        }

        public virtual void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            ObjInstance = objectInstance;
            PoolKey = poolKey;
        }

        public virtual void OnReturnObject()
        {
            gameObject.SetActive(false);
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(transform.position, parkingSpot.position);
        }
    }
}