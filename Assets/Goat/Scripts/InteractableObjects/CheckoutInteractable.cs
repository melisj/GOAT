using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI;
using Sirenix.OdinInspector;

namespace Goat.Grid.Interactions
{
    public class CheckoutInteractable : BaseInteractable
    {
        private List<Vector2Int> queueGridPositions = new List<Vector2Int>();
        private List<Vector3> queuePositions = new List<Vector3>();
        private Queue<Customer> customerQueue = new Queue<Customer>();

        [SerializeField, InteractableAttribute] private int maxQueue = 20;

        [SerializeField] Transform queueStartingPosition;
        [SerializeField] private bool overrideQueueDirection;
        [SerializeField, ShowIf("overrideQueueDirection")] private float queueStartingRotation;

        public int QueueLength => customerQueue.Count;

        public override object[] GetArgumentsForUI()
        {
            return new object[] { PeekCustomerFromQueue() };
        }

        public void AddCustomerToQueue(Customer customer)
        {
            if (customer != null && !customerQueue.Contains(customer))
                customerQueue.Enqueue(customer);
        }

        public Customer RemoveCustomerFromQueue()
        {
            return customerQueue.Dequeue();
        }

        public Customer PeekCustomerFromQueue()
        {
            if (customerQueue.Count == 0) return null; 
            return customerQueue.Peek();
        }

        protected void OnEnable()
        {
            StartCoroutine(GenerateQueue());
        }

        public void CreateQueue()
        {
            float rotY = transform.rotation.eulerAngles.y + (overrideQueueDirection ? queueStartingRotation : 0);
            float tileOffset = grid.GetTileSize / 2;

            Vector2Int gridStartPosition = grid.CalculateTilePositionInArray(queueStartingPosition.position);
            Vector2Int currentGridPosition = gridStartPosition;
            Vector2Int currentDirection = Vector2Int.zero;
            switch (((int)rotY + 360) % 360)
            {
                case 0: currentDirection = Vector2Int.down; break;
                case 90: currentDirection = Vector2Int.left; break;
                case 180: currentDirection = Vector2Int.up; break;
                case 270: currentDirection = Vector2Int.right; break;
            }

            while (queuePositions.Count < maxQueue)
            {
                queueGridPositions.Add(currentGridPosition);
                queuePositions.Add(new Vector3(currentGridPosition.x + tileOffset, 0, currentGridPosition.y + tileOffset));

                Vector2Int[] directions = { currentDirection, 
                    new Vector2Int(currentDirection.y, currentDirection.x),
                    -new Vector2Int(currentDirection.y, currentDirection.x)
                };

                Vector2Int tempGridPos = currentGridPosition;

                for(int i = 0; i < directions.Length; i++)
                {
                    if (CheckIfTileEmpty(currentGridPosition + directions[i]))
                    {
                        currentDirection = directions[i];
                        currentGridPosition += directions[i];
                        break;
                    }
                }

                if (tempGridPos == currentGridPosition)
                    break;
            }
        }

        private bool CheckIfTileEmpty(Vector2Int gridPosition)
        {
            Tile tile = grid.ReturnTile(gridPosition);
            if (tile == null) return false;
            return tile.IsEmpty; 
        }

        private IEnumerator GenerateQueue()
        {
            yield return new WaitForEndOfFrame();
            CreateQueue();
        }

        private void OnDrawGizmos()
        {
            for(int i = 0; i < queuePositions.Count; i++)
            {
                Gizmos.color = new Color(i / (float)queuePositions.Count, 0, 0);
                Gizmos.DrawSphere(queuePositions[i], 0.2f);
            }
        }
    }
}

