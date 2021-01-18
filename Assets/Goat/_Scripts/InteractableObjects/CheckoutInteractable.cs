using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI;
using Sirenix.OdinInspector;
using System.Linq;
using Goat.Saving;
using UnityAtoms.BaseAtoms;
using UnityAtoms;

namespace Goat.Grid.Interactions
{
    public class CheckoutInteractable : BaseInteractable, IAtomListener<Void>
    {
        // Queue lists
        private HashSet<Tile> queueTiles = new HashSet<Tile>();
        private List<Customer> customerQueue = new List<Customer>();

        [Required, SerializeField, TabGroup("References")] private VoidEvent onTileCreated;
        [Required, SerializeField, TabGroup("References")] private VoidEvent onTileDestroyed;
        [Required, SerializeField, TabGroup("References")] private VoidEvent onLevelLoaded;

        [Required, SerializeField, TabGroup("References")] private Transform queueStartingPosition;
        [Required, SerializeField, TabGroup("References")] private CheckoutChaChing chaChing;

        [Header("Queue Settings")]
        [SerializeField, TabGroup("Checkout")] private int maxQueue = 20;
        [SerializeField, TabGroup("Checkout")] private bool overrideQueueDirection;
        [SerializeField, TabGroup("Checkout"), ShowIf("overrideQueueDirection")] private float queueStartingRotation;
        [SerializeField, TabGroup("Checkout")] private LayerMask queueMask;

        // Properties
        public int PositionAmount => queueTiles.Count;
        public int QueueLength => customerQueue.Count;
        public bool QueueAvailable => customerQueue.Count < queueTiles.Count;
        public Vector3 LastPositionInQueue { get { return queueTiles.ElementAt(customerQueue.Count).Position; } }
        public bool Reachable { get; set; }


        // Direction data for the queue
        private List<Vector2Int> directionArray = new List<Vector2Int> { Vector2Int.down, Vector2Int.left, Vector2Int.up, Vector2Int.right };
        private Vector3 offsetFromGround = new Vector3(0, 0.2f, 0);

        public override object[] GetArgumentsForUI()
        {
            return new object[] { PeekCustomerFromQueue(), this };
        }

        #region Queue Behaviour

        public void AddCustomerToQueue(Customer customer)
        {
            if (customer != null && !customerQueue.Contains(customer) && QueueAvailable)
            {
                customerQueue.Add(customer);
                InvokeChange();
            }
        }

        [Button("Remove customer"), TabGroup("Checkout")]
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
                customerQueue[i].UpdatePositionInCheckoutQueue(queueTiles.ElementAt(i).Position);
            }
        }

        // Get the first customer in the queue
        public Customer PeekCustomerFromQueue()
        {
            if (customerQueue.Count == 0) return null;
            return customerQueue.First();
        }

        #endregion

        protected void OnEnable()
        {
            onLevelLoaded.RegisterSafe(this);
            onTileCreated.RegisterSafe(this);
            onTileDestroyed.RegisterSafe(this);
        }

        protected void OnDisable()
        {
            StopAllCoroutines();
            onLevelLoaded.UnregisterSafe(this);
            onTileCreated.UnregisterSafe(this);
            onTileDestroyed.UnregisterSafe(this);
        }

        public void OnEventRaised(Void item)
        {
            StartGeneration();
        }

        #region Queue Generation

        private void StartGeneration()
        {
            StartCoroutine(GenerateQueue());
        }

        // Create a path for the queue
        private void CreateQueue()
        {
            // Get the starting tile with rotation which this checkout is faceing
            float rotY = transform.rotation.eulerAngles.y + (overrideQueueDirection ? queueStartingRotation : 0);
            Tile startingTile = Grid.ReturnTile(Grid.CalculateTilePositionInArray(queueStartingPosition.position));

            // Check if queue can be generated from starting point (is queue obstructed)
            Reachable = !Physics.Raycast(transform.position + offsetFromGround, queueStartingPosition.position - transform.position, 1, queueMask);
            if (!Reachable) { Debug.LogWarning("Unreachable queue created!", gameObject); return; }

            // Check direction the queue should start growing towards
            Vector2Int currentDirection = directionArray[((int)rotY + 360) % 360 / 90];

            // Queue can snake 5 times before stopping
            int deviations = 0, maxDeviations = 5;

            // Add queue spots
            while (queueTiles.Count < maxQueue)
            {
                Vector2Int[] directions = { currentDirection,
                    new Vector2Int(currentDirection.y, currentDirection.x), // Relativly left
                    -new Vector2Int(currentDirection.y, currentDirection.x) // Relativly right
                };

                Tile previousTile = startingTile;
                List<Tile> longestPath = new List<Tile>();

                // Check for each directions
                for (int i = 0; i < directions.Length; i++)
                {
                    // Get path from direction
                    ReturnTilesTillObstruction(previousTile, directions[i], longestPath.Count, out List<Tile> tiles);
                    if (tiles.Count > longestPath.Count)
                    {
                        longestPath = tiles;
                        currentDirection = directions[i];
                        startingTile = tiles[tiles.Count - 1];

                        // Stop searching for other paths if the first one is valid (only when first searching for path)
                        if (queueTiles.Count == 0 && tiles.Count > 0)
                            break;
                    }
                }
                foreach (Tile tile in longestPath)
                {
                    if(queueTiles.Count < maxQueue)
                        queueTiles.Add(tile);
                }


                // Stop if same tile
                if (previousTile == startingTile)
                    break;

                // Stop when max deviations has been reached (queue has most likely terminated and can't find any new tiles)
                if (deviations > maxDeviations)
                    break;
                deviations++;
            }
        }

        // Get tile info and check if it has a floor and not a building
        private void ReturnTilesTillObstruction(Tile currentTile, Vector2Int direction, float currentLongestDistance, out List<Tile> tiles)
        {
            tiles = new List<Tile>();

            Vector3 startPosition = currentTile.Position + offsetFromGround;
            Vector3 startDirection = new Vector3(direction.x, 0, direction.y);

            if (Physics.Raycast(startPosition, startDirection, out RaycastHit hit, float.MaxValue, queueMask))
            {
                if (hit.distance > currentLongestDistance)
                {
                    int tilesToGet = Mathf.FloorToInt(hit.distance);
                    for(int i = 0; i <= tilesToGet; i++)
                    {
                        Tile checkedTile = Grid.ReturnTile(currentTile.TilePosition + direction * i);   
                        if(checkedTile != null && checkedTile.FloorObj != null)
                            tiles.Add(checkedTile);
                    }
                }
            }
        }

        // Start queue creation after the frame it was enabled
        private IEnumerator GenerateQueue()
        {
            queueTiles.Clear();
            // Wait a second for every animations playing
            yield return new WaitForSeconds(1f);
            CreateQueue();
        }

        #endregion

        private void OnDrawGizmos()
        {
            for (int i = 0; i < queueTiles.Count; i++)
            {
                Gizmos.color = new Color(i / (float)queueTiles.Count, 0, 0);
                Gizmos.DrawSphere(queueTiles.ElementAt(i).Position, 0.3f);
            }
        }

    }
}