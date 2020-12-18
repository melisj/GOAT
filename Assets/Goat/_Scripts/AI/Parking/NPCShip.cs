using Goat.Pooling;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Goat.AI.Parking
{
    public class NPCShip : MonoBehaviour, IPoolObject
    {
        private ParkingSpots.ParkingSpot parkingSpot;
        public ParkingSpots.ParkingSpot ParkingSpot { 
            get { return parkingSpot; } 
            set { parkingSpot = value; LandShip(); } 
        }

        public Queue<Vector3> flightPath = new Queue<Vector3>();
        public Queue<float> accuracyOnPath = new Queue<float>();

        public NPCSpawner NpcSpawner { get; set; }
        public ShipSpawner Spawner { get; set; }

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

        private void ShipHasLanded()
        {
            // Spawn NPC
            NpcSpawner.SpawnNPC();
        }

        public void ShipReadyToFly()
        {
            // Customer has arrived
            parkingSpot.ocupied = false;
            AddFlightPath(transform.position + new Vector3(0, Spawner.ArrivalHeight, 0), 1);
            AddFlightPath(transform.position + transform.forward * 100, 20);
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

        public void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            ObjInstance = objectInstance;
            PoolKey = poolKey;
        }

        public void OnReturnObject()
        {
            gameObject.SetActive(false);
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(transform.position, parkingSpot.position);
        }
    }
}