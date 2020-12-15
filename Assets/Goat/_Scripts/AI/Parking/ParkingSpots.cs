using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Goat.AI.Parking
{

    public class ParkingSpots : MonoBehaviour
    {
        [SerializeField] private Vector2Int parkingRows;
        [SerializeField, ReadOnly] private int parkingAmount;
        [SerializeField] private ParkingSpot[] parkingList;

        private Bounds bounds;

        private void OnValidate()
        {
            parkingRows.x = Mathf.Clamp(parkingRows.x, 0, 50);
            parkingRows.y = Mathf.Clamp(parkingRows.y, 0, 50);

            parkingAmount = parkingRows.x * parkingRows.y;

            StartGeneration();
        }

        private void Awake()
        {
            StartGeneration();
        }

        #region Generation

        [Button("Generate Parking", ButtonSizes.Large)]
        private void StartGeneration()
        {
            parkingList = new ParkingSpot[parkingAmount];

            if (GetBounds())
                GenerateParkingSpaces();
            else
                Debug.LogError("No parking spaces could be created, missing both a collider and a meshrenderer!");
        }

        private bool GetBounds()
        {
            Collider collider = GetComponent<Collider>();
            if (collider) bounds = collider.bounds;

            if (bounds == null)
            {
                MeshRenderer renderer = GetComponent<MeshRenderer>();
                if (renderer) bounds = renderer.bounds;
            }

            return bounds != null;
        }

        private void GenerateParkingSpaces()
        {
            float offsetX = bounds.size.x / (parkingRows.x + 1);
            float offsetY = bounds.size.z / (parkingRows.y + 1);

            for (int x = 0, i = 0; x < parkingRows.x; x++)
            {
                for (int y = 0; y < parkingRows.y; y++, i++)
                {
                    parkingList[i] = new ParkingSpot(
                        bounds.min + new Vector3(offsetX * (x + 1), 0, offsetY * (y + 1)), i);
                }
            }
        }

        #endregion

        #region Managing Spaces

        public ParkingSpot GetRandomParkingSpot()
        {
            IEnumerable<ParkingSpot> enumerator = parkingList.Where(x => x.ocupied == false);
            if (enumerator.Count() != 0)
            {
                int randomSpot = Random.Range(0, enumerator.Count());
                ParkingSpot spot = enumerator.ElementAt(randomSpot);
                spot.ocupied = true;

                return spot;
            }
            return null;
        }


        public ParkingSpot GetFirstAvailableParkingSpot()
        {
            ParkingSpot spot = parkingList.First(x => x.ocupied == false);
            if (spot != null)
            {
                spot.ocupied = true;
                return spot;
            }

            return null;
        }

        public void FreeParkingSpot(ParkingSpot spot)
        {
            parkingList[spot.arrayPos].ocupied = false;
        }

        #endregion

        #region Gizmos 

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            foreach (ParkingSpot location in parkingList)
            {
                Gizmos.DrawCube(location.position, Vector3.one);
            }
        }

        #endregion

        [System.Serializable]
        public class ParkingSpot
        {

            public Vector3 position;
            public int arrayPos;
            public bool ocupied;

            public ParkingSpot(Vector3 position, int arrayPos)
            {
                this.position = position;
                this.arrayPos = arrayPos;
                this.ocupied = false;
            }
        }
    }
}