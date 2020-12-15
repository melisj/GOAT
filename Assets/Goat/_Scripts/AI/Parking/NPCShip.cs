using Goat.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.AI.Parking
{
    public class NPCShip : MonoBehaviour, IPoolObject
    {
        [SerializeField] private Parking parkingInfo;
        private ParkingSpots.ParkingSpot parkingSpot;

        public int PoolKey { get; set; }
        public ObjectInstance ObjInstance { get; set; }

        void Start()
        {
            parkingSpot = parkingInfo.CurrentSpotSpawing;
            LandShip();
        }

        void Update()
        {

        }

        private void ShipHasLanded()
        {
            print("The eagle has landed");
        }

        private void ShipReadyToFly()
        {

        }

        private void ShipHasDeparted()
        {

        }

        private void LandShip()
        {
            StartCoroutine(Landing());
        }

        private IEnumerator Landing()
        {
            while (!(Mathf.Abs(transform.position.y - parkingSpot.position.y) < 0.1f))
            {
                transform.position = Vector3.Slerp(transform.position, parkingSpot.position, Time.deltaTime);

                yield return null;
            }

            ShipHasLanded();
        }

        public void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
        }

        public void OnReturnObject()
        {
        }
    }
}