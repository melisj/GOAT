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

namespace Goat.Farming
{
    public class TubeManager : MonoBehaviour, IAtomListener<GameObject>
    {
        private HashSet<TubeDirection> checkedTubes;

        [SerializeField] private FarmNetworkData networkData;

        [SerializeField] private GameObjectEvent onGridChange;
        [SerializeField] private GameObjectEvent onTubeEndNeeded;
        [SerializeField] private TubeDirectionEvent tubeDirectionEvent;

        private void OnEnable()
        {
            onTubeEndNeeded.RegisterSafe(this);
            onGridChange.RegisterSafe(ConnectNetwork);
        }

        private void OnDisable()
        {
            onTubeEndNeeded.UnregisterSafe(this);
            onGridChange.UnregisterSafe(ConnectNetwork);
        }

        private void ConnectNetwork(GameObject nothing)
        {
            StartNetworkConnection();
        }

        #region Network Setup

        private void InitNetworkData()
        {
            checkedTubes = new HashSet<TubeDirection>();

            for (int i =0; i < networkData.Pipes.Count; i++)
            {
                networkData.Pipes[i].ConnectedMultiDirections = new TubeDirection[networkData.Pipes[i].ConnectionAmount];
                networkData.Pipes[i].DistanceTillNextDirection = new int[networkData.Pipes[i].ConnectionAmount];
            }
        }

        [Button("Connect network test")]
        private void StartNetworkConnection()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            InitNetworkData();
            ConnectNetwork();
            SearchForEachFarm();
            timer.Stop();
            Debug.Log(timer.Elapsed);
        }

        private void ConnectNetwork()
        {
            for (int i = 0; i < networkData.Farms.Count; i++)
            {
                TubeDirection startTube = networkData.Farms[i].GetComponent<TubeDirection>();
                if (startTube.ConnectedTubes.Count != 0)
                {
                    ConnectFrom(startTube, null, out int arrivalIndex, out int distance);
                }
            }
        }

        private TubeDirection ConnectFrom(TubeDirection startTube, TubeDirection prevTube, out int arrivalIndex, out int distance)
        {
            TubeDirection currentTube = startTube;
            TubeDirection previousTube = prevTube;
            arrivalIndex = 0; // Index of the incoming pipe when following the path
            distance = 0;

            int maxIter = 1000, iter = 0;
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
                                }
                                catch (Exception e)
                                {
                                    Debug.LogError(currentTube + " - " + i, currentTube);
                                }

                                // Assigns references to the found connection
                                if (foundTube && foundTube.ConnectedMultiDirections.Length > foundIndex)
                                {
                                    foundTube.ConnectedMultiDirections[foundIndex] = currentTube;
                                    foundTube.DistanceTillNextDirection[foundIndex] = foundDistance;
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

        #endregion

        #region PathFinding

        private void SearchForEachFarm()
        {
            for (int i = 0; i < networkData.Farms.Count; i++)
            {
                networkData.Farms[i].FindTubeEnd();
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

            int maxIter = 1000, iter = 0;

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
                if (iter > maxIter) { Debug.LogWarning("Tube manager has done too many iterations"); return null; }
                iter++;

            } while (tubesToCheck.Count > 0);

            return null;
        }

        public void OnEventRaised(GameObject item)
        {
            TubeDirection farmTubeDir = item.GetComponent<TubeDirection>();
            tubeDirectionEvent.Raise(new WithOwner<TubeDirection>(SearchForTubeEndDijkstra(farmTubeDir), item));
        }

        #endregion
    }

}