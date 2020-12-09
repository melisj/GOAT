using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI;
using Sirenix.OdinInspector;
using System.Linq;

namespace Goat.Grid.Interactions
{
    public class CheckoutInteractable : BaseInteractable
    {
        private List<Vector2Int> queueGridPositions = new List<Vector2Int>();
        private List<Vector3> queuePositions = new List<Vector3>();
        private List<Customer> customerQueue = new List<Customer>();

        [SerializeField, InteractableAttribute] private int maxQueue = 20;

        [SerializeField] Transform queueStartingPosition;
        [SerializeField] private bool overrideQueueDirection;
        [SerializeField, ShowIf("overrideQueueDirection")] private float queueStartingRotation;

        public int QueueLength => customerQueue.Count;
        public bool QueueAvailable => customerQueue.Count < queuePositions.Count;
        public Vector3 LastPositionInQueue { get { return queuePositions[customerQueue.Count];  } }

        public override object[] GetArgumentsForUI()
        {
            return new object[] { PeekCustomerFromQueue() };
        }

        public void AddCustomerToQueue(Customer customer)
        {
            if (customer != null && !customerQueue.Contains(customer))
                customerQueue.Add(customer);
        }

        public void RemoveCustomerFromQueue()
        {
            if (customerQueue.Count > 0)
                customerQueue.Remove(customerQueue.First());

            UpdateCustomersInQueue();
        }

        private void UpdateCustomersInQueue()
        {
            for (int i = 0; i < customerQueue.Count; i++)
            {
                customerQueue[i].UpdatePositionInCheckoutQueue(queuePositions[i]);
            }
        }

        public Customer PeekCustomerFromQueue()
        {
            if (customerQueue.Count == 0) return null; 
            return customerQueue.First();
        }

        protected void OnEnable()
        {
            StartCoroutine(GenerateQueue());
        }

        public void CreateQueue()
        {
            queueGridPositions.Clear();
            queuePositions.Clear();

            float rotY = transform.rotation.eulerAngles.y + (overrideQueueDirection ? queueStartingRotation : 0);
            float tileOffset = grid.GetTileSize / 2;

            Vector2Int gridStartPosition = grid.CalculateTilePositionInArray(queueStartingPosition.position);
            Vector2Int currentGridPosition = gridStartPosition;
            Vector2Int currentDirection = Vector2Int.zero;

            // Check direction the queue should start growing towards
            switch (((int)rotY + 360) % 360)
            {
                case 0: currentDirection = Vector2Int.down; break;
                case 90: currentDirection = Vector2Int.left; break;
                case 180: currentDirection = Vector2Int.up; break;
                case 270: currentDirection = Vector2Int.right; break;
            }

            // Add queue spots
            while (queuePositions.Count < maxQueue)
            {
                queueGridPositions.Add(currentGridPosition);
                queuePositions.Add(new Vector3(currentGridPosition.x + tileOffset, 0, currentGridPosition.y + tileOffset));

                Vector2Int[] directions = { currentDirection,
                    new Vector2Int(currentDirection.y, currentDirection.x), // Relativly right
                    -new Vector2Int(currentDirection.y, currentDirection.x) // Relativly left
                };

                Vector2Int tempGridPos = currentGridPosition;

                // Check for each directions 
                for(int i = 0; i < directions.Length; i++)
                {
                    if (CheckIfTileEmpty(currentGridPosition + directions[i]))
                    {
                        currentDirection = directions[i];
                        currentGridPosition += directions[i];
                        break;
                    }
                }

                // Stop if same tile
                if (tempGridPos == currentGridPosition)
                    break;
            }
        }

        // Get tile info and check if it has a floor and not a building
        private bool CheckIfTileEmpty(Vector2Int gridPosition)
        {
            Tile tile = grid.ReturnTile(gridPosition);
            if (tile == null) return false;
            return tile.IsEmpty; 
        }

        // Start queue creation after the frame it was enabled
        private IEnumerator GenerateQueue()
        {
            yield return new WaitForEndOfFrame();
            CreateQueue();
        }

        private void OnDrawGizmos()
        {
            for(int i = 0; i < queueGridPositions.Count; i++)
            {
                Gizmos.color = new Color(i / (float)queuePositions.Count, 0, 0);
                Gizmos.DrawSphere(queuePositions[i], 0.2f);
            }
        }
    }
}

