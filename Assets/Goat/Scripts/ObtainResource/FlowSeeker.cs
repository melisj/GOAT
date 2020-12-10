using Goat.Farming;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Seeks the end object
/// And adds this to the path of the farm
/// </summary>
public class FlowSeeker : MonoBehaviour
{
    [SerializeField] private FarmStationFunction farmStation;
    [SerializeField] private List<TubeDirection> collidedTubes;
    [SerializeField] private Path pathsv3;
    [SerializeField] private List<Path> paths;
    [SerializeField] private Color[] colors;
    [SerializeField] private List<Vector3> nodes;
    private HashSet<TubeDirection> endTubes;
    public List<TubeDirection> CollidedTubes => collidedTubes;

    [Button]
    private void CheckForAllSources()
    {
        endTubes = new HashSet<TubeDirection>();
        paths = new List<Path>();
        endTubes.Clear();
        paths.Clear();
        RandomColors();

        FindAll();
    }

    private void RandomColors()
    {
        colors = new Color[99];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Random.ColorHSV(0, 1);
            colors[i].a = 1f;
        }
    }

    [Button]
    private void CreateGraph()
    {
        HashSet<TubeDirection> checkedSet = new HashSet<TubeDirection>(collidedTubes);
        Stack<TubeDirection> remainingStack = new Stack<TubeDirection>(collidedTubes);
        nodes = new List<Vector3>();

        while (remainingStack.Count > 0)
        {
            TubeDirection currentNode = remainingStack.Pop();

            for (int i = 0; i < currentNode.ConnectedTubes.Length; i++)
            {
                TubeDirection nextNode = currentNode.ConnectedTubes[i];
                if (!nextNode) continue;

                if (checkedSet.Add(nextNode))
                {
                    nodes.Add(currentNode.CorrectPosWithRotation(currentNode.Offset[i]));
                    remainingStack.Push(currentNode);
                }
            }
        }
    }

    public bool FindAll()
    {
        //Prep collections with this object's connections
        bool found = false;
        pathsv3.Points = new List<Vector3>();
        var checkedSet = new HashSet<TubeDirection>(collidedTubes);
        var remainingStack = new Stack<TubeDirection>(collidedTubes);
        TubeDirection prevTube = null;
        AddPath();
        //Are there items left to check?
        for (int i = 0; i < paths.Count; i++)
        {
            FindOne(checkedSet, remainingStack, ref prevTube, i);
            pathsv3.Points.Clear();
            //remainingStack = new Stack<TubeDirection>(checkedSet);
        }
        return found;
    }

    private bool FindOne(HashSet<TubeDirection> checkedSet, Stack<TubeDirection> remainingStack, ref TubeDirection prevTube, int index)
    {
        while (remainingStack.Count > 0)
        {
            //Reference the next item and remove it from remaining
            var item = remainingStack.Pop();
            TubeDirection currentTube = item.GetComponent<TubeDirection>();
            //for (int i = 0; i < currentTube.ConnectedTubes.Length; i++)
            //{
            //    if (currentTube.ConnectedTubes[i] == null) continue;
            //    AddPathPoints(currentTube, i);
            //}
            //If it's the source, we're done
            if (item.CompareTag("TubeEnd"))
            {
                if (endTubes.Add(item))
                {
                    EditPath(index);
                    TubeEnd tubeEnd = item.GetComponent<TubeEnd>();
                    if (tubeEnd)
                    {
                        tubeEnd.ConnectedFarms.Add(farmStation);
                    }
                    Debug.Log($"Adding new endtube {endTubes}, added new path {paths.Count}");
                    return true;
                }
                else
                {
                    Debug.Log($"Already added endtube {item}, gotta find a new one");
                }
                Debug.Log($"Done with adding {paths.Count}");
            }
            else
            {
                for (int i = 0; i < currentTube.ConnectedTubes.Length; i++)
                {
                    TubeDirection newTube = currentTube.ConnectedTubes[i];
                    if (!newTube) continue;
                    if (checkedSet.Add(newTube))
                    {
                        AddPathPoints(currentTube, i);
                        int pathCount = newTube.GetPathCount();
                        if (pathCount > 0)
                        {//Add new paths before the multi
                            for (int y = 0; y < pathCount; y++)
                            {
                                AddPath();
                            }
                        }

                        remainingStack.Push(newTube);
                    }
                }
                prevTube = currentTube;
            }
        }

        return false;
    }

    private void AddPathPoints(TubeDirection currentTube, int i)
    {
        for (int j = 0; j < currentTube.GetPath(i - 1).Points.Count; j++)
        {
            if (j == 0)
            {
                if (currentTube.ConnectedTubes[0] == null) continue;
            }
            currentTube.GetPath(i - 1).Points.Rotate((int)(currentTube.transform.rotation.y / 90));
            Vector3 pos = currentTube.CorrectPosWithRotation(currentTube.GetPath(i - 1).Points[j]);
            if (!pathsv3.Points.Contains(pos))
            {
                pathsv3.Points.Add(pos);
            }
        }
    }

    private void AddPath()
    {
        Path newPath = new Path();
        newPath.Points = new List<Vector3>(pathsv3.Points);
        paths.Add(newPath);
    }

    private void EditPath(int index)
    {
        if (index < paths.Count)
        {
            paths[index].Points.AddRange(pathsv3.Points);
        }
    }

    private void DrawNodes()
    {
        if (nodes == null) return;
        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 pos = nodes[i];
            pos.y = 1f;
            Gizmos.DrawSphere(pos, 0.1f);
        }
    }

    private void OnDrawGizmos()
    {
        DrawNodes();
        if (paths == null) return;

        for (int i = 0; i < paths.Count; i++)
        {
            if (paths[i].Points == null) continue;
            Gizmos.color = colors == null || colors[i] == null ? Color.yellow : colors[i];
            for (int j = 0; j < paths[i].Points.Count; j++)
            {
                float posY = (i * 0.25f) + 0.75f;
                Vector3 pos = paths[i].Points[j];
                pos.y = posY;
                if (j <= 0)
                {
                    Gizmos.DrawLine(transform.position, pos);
                    Gizmos.DrawSphere(pos, 0.1f);
                }
                else if (j < paths[i].Points.Count)
                {
                    Vector3 posMin = paths[i].Points[j - 1];
                    posMin.y = posY;
                    Gizmos.DrawSphere(pos, 0.1f);

                    Gizmos.DrawLine(posMin, pos);
                }
            }
        }
    }
}