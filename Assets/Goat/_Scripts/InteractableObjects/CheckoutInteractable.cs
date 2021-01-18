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
        // Queue lists
        private List<Vector2Int> queueGridPositions = new List<Vector2Int>();
        private List<Vector3> queuePositions = new List<Vector3>();
        private List<Customer> customerQueue = new List<Customer>();
        [Header("Queue Settings")]
        [SerializeField] private int maxQueue = 20;
        [SerializeField] private Transform queueStartingPosition;
        [SerializeField] private bool overrideQueueDirection;
        [SerializeField, ShowIf("overrideQueueDirection")] private float queueStartingRotation;

        // Properties
        public int PositionAmount => queuePositions.Count;

        public int QueueLength => customerQueue.Count;
        public bool QueueAvailable => customerQueue.Count < queuePositions.Count;
        public Vector3 LastPositionInQueue { get { return queuePositions[customerQueue.Count]; } }

        // Direction data for the queue
        private List<Vector2Int> directionArray = new List<Vector2Int> { Vector2Int.down, Vector2Int.left, Vector2Int.up, Vector2Int.right };

        [SerializeField] private CheckoutChaChing chaChing;

        public override object[] GetArgumentsForUI()
        {
            return new object[] { PeekCustomerFromQueue(), this };
        }

        public void AddCustomerToQueue(Customer customer)
        {
            if (customer != null && !customerQueue.Contains(customer) && QueueAvailable)
            {
                customerQueue.Add(customer);
                InvokeChange();
            }
        }

        [Button("Remove customer")]
        public void RemoveCustomerFromQueue()
        {
            StartCoroutine(RemoveEndOfFrame());
        }

        // [BUG REPORT] https://trello.com/c/ipURDq80/141-bug-button-event-in-ui-for-removing-customer-from-queue-not-working-as-it-should
        // Dirty fix for catching button UI event and handling event after the frame
        private IEnumerator RemoveEndOfFrame()
        {
            yield return new WaitForEndOfFrame();

            if (customerQueue.Count > 0)
            {
                chaChing.PlayAudio();

                customerQueue.First().LeaveStore();
                customerQueue.RemoveAt(0);

                UpdateCustomersInQueue();

                InvokeChange();
            }
        }

        // Move the queue along
        private void UpdateCustomersInQueue()
        {
            for (int i = 0; i < customerQueue.Count; i++)
            {
                customerQueue[i].UpdatePositionInCheckoutQueue(queuePositions[i]);
            }
        }

        // Get the first customer in the queue
        public Customer PeekCustomerFromQueue()
        {
            if (customerQueue.Count == 0) return null;
            return customerQueue.First();
        }

        protected void OnEnable()
        {
            StartCoroutine(GenerateQueue());
        }

        protected void OnDisable()
        {
            StopAllCoroutines();
        }

        // Create a path for the queue
        public void CreateQueue()
        {
            queueGridPositions.Clear();
            queuePositions.Clear();

            float rotY = transform.rotation.eulerAngles.y + (overrideQueueDirection ? queueStartingRotation : 0);
            float tileOffset = grid.GetTileSize / 2;

            Vector2Int gridStartPosition = grid.CalculateTilePositionInArray(queueStartingPosition.position);
            Vector2Int currentGridPosition = gridStartPosition;

            // Check direction the queue should start growing towards
            Vector2Int currentDirection = directionArray[((int)rotY + 360) % 360 / 90];

            // Add queue spots
            while (queuePositions.Count < maxQueue)
            {
                queueGridPositions.Add(currentGridPosition);
                queuePositions.Add(new Vector3(currentGridPosition.x + tileOffset, 0, currentGridPosition.y + tileOffset));

                Vector2Int[] directions = { currentDirection,
                    new Vector2Int(currentDirection.y, currentDirection.x), // Relativly left
                    -new Vector2Int(currentDirection.y, currentDirection.x) // Relativly right
                };

                Vector2Int tempGridPos = currentGridPosition;

                // Check for each directions
                for (int i = 0; i < directions.Length; i++)
                {
                    if (CheckIfTileEmpty(currentGridPosition, directions[i]))
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
        private bool CheckIfTileEmpty(Vector2Int currentPosition, Vector2Int direction)
        {
            Tile oldTile = grid.ReturnTile(currentPosition);
            Tile newTile = grid.ReturnTile(currentPosition + direction);
            if (newTile == null) return false;

            return newTile.IsEmpty &&
                !newTile.HasWallOnSide(directionArray.IndexOf(-direction)) &&
                !oldTile.HasWallOnSide(directionArray.IndexOf(direction));
        }

        // Start queue creation after the frame it was enabled
        private IEnumerator GenerateQueue()
        {
            yield return new WaitForEndOfFrame();
            CreateQueue();
        }

        protected override void IsClicked(Transform clickedObj)
        {
            base.IsClicked(clickedObj);
        }

        private void OnDrawGizmos()
        {
            for (int i = 0; i < queueGridPositions.Count; i++)
            {
                Gizmos.color = new Color(i / (float)queuePositions.Count, 0, 0);
                Gizmos.DrawSphere(queuePositions[i], 0.2f);
            }
        }
    }
}