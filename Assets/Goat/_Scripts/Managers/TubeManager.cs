using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine;
using System;
using System.Linq;
using UnityAtoms.BaseAtoms;
using UnityAtoms;
using System.Threading;

namespace Goat.Farming
{
    public class TubeManager : MonoBehaviour, IAtomListener<GameObject>
    {
        private HashSet<TubeDirection> checkedTubes;

        [SerializeField] private FarmNetworkData networkData;

        [SerializeField] private GameObjectEvent onGridChange;
        [SerializeField] private GameObjectEvent onTubeEndNeeded;
        [SerializeField] private TubeDirectionEvent tubeDirectionEvent;

        private Thread connectionThread;
        private bool networkChanged = false;

        [Header("Debug")]
        // Debug for connection function
        [SerializeField] private int debugI;
        [SerializeField] private TubeDirection debugSelected;

        private void OnEnable()
        {
            onTubeEndNeeded.RegisterSafe(this);
            onGridChange.RegisterSafe(ConnectNetwork);

            StartCoroutine(StartNetworkConnection());
        }

        private void OnDisable()
        {
            onTubeEndNeeded.UnregisterSafe(this);
            onGridChange.UnregisterSafe(ConnectNetwork);

            StopAllCoroutines();
            if (connectionThread != null && connectionThread.IsAlive)
                connectionThread.Abort();
        }

        private void ConnectNetwork(GameObject nothing)
        {
            networkChanged = true;
        }

        #region Network Setup

        // Reset network data
        private void InitNetworkData()
        {
            checkedTubes = new HashSet<TubeDirection>();

            for (int i = 0; i < networkData.Pipes.Count; i++)
            {
                networkData.Pipes[i].ConnectedMultiDirections = new TubeDirection[networkData.Pipes[i].ConnectionAmount];
                networkData.Pipes[i].DistanceTillNextDirection = new int[networkData.Pipes[i].ConnectionAmount];
            }
        }

        private IEnumerator StartNetworkConnection()
        {
            while (true)
            {
                if (networkChanged)
                {
                    // Stop thread if busy
                    if (connectionThread != null && connectionThread.IsAlive)
                        connectionThread.Abort();

                    // Connect all tubes together
                    connectionThread = new Thread(ConnectNetwork);
                    connectionThread.Start();

                    yield return new WaitUntil(() => !connectionThread.IsAlive);

                    // Connect
                    connectionThread = new Thread(SearchForEachFarm);
                    connectionThread.Start();

                    yield return new WaitUntil(() => !connectionThread.IsAlive);

                    Debug.Log("Done checking network!");
                    networkChanged = false;
                }

                yield return new WaitForSeconds(1);
            }
        }

        private void ConnectNetwork()
        {
            InitNetworkData();

            for (int i = 0; i < networkData.Pipes.Count; i++)
            {
                if (networkData.Pipes[i].IsFarmStation)
                {
                    if (networkData.Pipes[i].ConnectedTubes.Count != 0)
                    {
                        ConnectFrom(networkData.Pipes[i], null, out int arrivalIndex, out int distance);
                    }
                }
            }
        }

        private TubeDirection ConnectFrom(TubeDirection startTube, TubeDirection prevTube, out int arrivalIndex, out int distance)
        {
            TubeDirection currentTube = startTube;
            TubeDirection previousTube = prevTube;
            arrivalIndex = 0; // Index of the incoming pipe when following the path
            distance = 0;

            int maxIter = 10000, iter = 0;
            do
            {
                distance++;

                if (currentTube.ExchangePoint)
                {
                    if (checkedTubes.Add(currentTube))
                    {
                        for (int i = 0; i < currentTube.ConnectedTubes.Count; i++)
                        {
                            if (currentTube.ConnectedTubes[i] != previousTube)
                            {
                                // Recursive function <returns>distance to next connection</returns>
                                TubeDirection foundTube = ConnectFrom(currentTube.ConnectedTubes[i], currentTube, out int foundIndex, out int foundDistance);

                                // Assigns references for this tube
                                try
                                {
                                    currentTube.ConnectedMultiDirections[i] = foundTube;
                                    currentTube.DistanceTillNextDirection[i] = foundDistance;

                                    // Assigns references to the found connection
                                    if (foundTube && foundTube.ConnectedMultiDirections.Length > foundIndex)
                                    {
                                        foundTube.ConnectedMultiDirections[foundIndex] = currentTube;
                                        foundTube.DistanceTillNextDirection[foundIndex] = foundDistance;
                                    }
                                }
                                catch (Exception e)
                                {
                                    debugI = i;
                                    debugSelected = currentTube;
                                    throw e;
                                }
                            }
                        }

                        // Return on end points
                        if (currentTube.EndPoint)
                            return currentTube;
                    }

                    // Out index of incoming path in current tube
                    arrivalIndex = currentTube.ConnectedTubes.IndexOf(previousTube);
                    return currentTube;
                }
                else
                {
                    // For dead ends
                    if (currentTube.ConnectedTubes.Count <= 1)
                        return null;

                    // Check for next tube
                    for (int i = 0; i < currentTube.ConnectedTubes.Count; i++)
                    {
                        if (currentTube.ConnectedTubes[i] != previousTube)
                        {
                            previousTube = currentTube;
                            currentTube = currentTube.ConnectedTubes[i];
                            break;
                        }
                    }
                }

                // Safety
                if (iter > maxIter) { Debug.LogWarning("Tube manager has done too many iterations"); return null; }
                iter++;
            }
            while (currentTube != null);
            return null;
        }

        #endregion Network Setup

        #region PathFinding

        private void SearchForEachFarm()
        {
            for (int i = 0; i < networkData.Farms.Count; i++)
            {
                //networkData.Farms[i].FindTubeEnd();
                networkData.Farms[i].FoundTubeEnd = SearchForTubeEndDijkstra(networkData.Farms[i].TubeDirection);
            }
        }

        private void ResetTubesForPathfinding()
        {
            for (int i = 0; i < networkData.Pipes.Count; i++)
            {
                networkData.Pipes[i].VisitedByAlgorithm = false;
                networkData.Pipes[i].DistanceFromStart = 0;
            }
        }

        public TubeDirection SearchForTubeEndDijkstra(TubeDirection startingTube)
        {
            ResetTubesForPathfinding();

            List<TubeDirection> tubesToCheck = new List<TubeDirection>();
            tubesToCheck.Add(startingTube);

            int maxIter = 10000, iter = 0;

            do
            {
                // Order the list of all the nodes that are uncovered by the algorithm
                tubesToCheck = tubesToCheck.OrderBy(tube => (tube.DistanceFromStart)).ToList();
                TubeDirection currentTube = tubesToCheck[0];
                tubesToCheck.Remove(currentTube);

                // Check if this is an end node
                if (currentTube.EndPoint)
                {
                    return currentTube;
                }

                if (currentTube != null)
                {
                    // Check each connection of the current shortest distance node
                    for (int IConnection = 0; IConnection < currentTube.ConnectedMultiDirections.Length; IConnection++)
                    {
                        // Dont check this node when it has already been checked
                        if (currentTube.ConnectedMultiDirections[IConnection] == null || currentTube.VisitedByAlgorithm)
                            continue;

                        TubeDirection nextTube = currentTube.ConnectedMultiDirections[IConnection];

                        // Check if the childNode CostFromStart is smaller than the new connection we are checking
                        if (nextTube.DistanceFromStart == 0 || currentTube.DistanceFromStart + currentTube.DistanceTillNextDirection[IConnection] < nextTube.DistanceFromStart)
                        {
                            // Add the cost from start to the node
                            nextTube.DistanceFromStart = currentTube.DistanceFromStart + currentTube.DistanceTillNextDirection[IConnection];

                            // Add this childNode when it was not already checked by the algorithm
                            if (!tubesToCheck.Contains(nextTube))
                                tubesToCheck.Add(nextTube);
                        }
                    }
                    currentTube.VisitedByAlgorithm = true;
                }

                // Safety
                if (iter > maxIter) { Debug.LogWarning("Dijkstra has done too many iterations"); return null; }
                iter++;
            } while (tubesToCheck.Count > 0);

            return null;
        }

        public void OnEventRaised(GameObject item)
        {
            TubeDirection farmTubeDir = item.GetComponent<TubeDirection>();
            tubeDirectionEvent.Raise(new WithOwner<TubeDirection>(SearchForTubeEndDijkstra(farmTubeDir), item));
        }

        #endregion PathFinding
    }
}