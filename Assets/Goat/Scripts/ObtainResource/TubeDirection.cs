using Goat.Events;
using Goat.Pooling;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityAtoms;
using UnityEngine;

namespace Goat.Farming
{
    public class TubeDirection : EventListenerVoid, IPoolObject
    {
        [SerializeField] private FarmStationFunction connectedFarm;
        [SerializeField] private bool multiDirection;
        [SerializeField, HideIf("multiDirection")] protected Vector3[] positions;
        [SerializeField, ShowIf("multiDirection")] private Path[] Paths;
        [SerializeField] private LayerMask layer;
        //[SerializeField] private Vector3 rayDirection;
        //[SerializeField] private float rayDistance;
        [SerializeField] private Vector3[] offset;
        [SerializeField] private float radius = 0.2f;
        private int similarIndex;
        private int offsetIndex;
        private TubeDirection previousTube;
        private bool adjustingTube;

        public int PathIndex { get; set; }
        public int PoolKey { get; set; }
        public ObjectInstance ObjInstance { get; set; }
        public FarmStationFunction ConnectedFarm { get => connectedFarm; set => connectedFarm = value; }
        public Vector3[] Offset => offset;

        public bool MultiDirection => multiDirection;

        public void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            OnGridChange();
            ObjInstance = objectInstance;
            PoolKey = poolKey;
        }

        private int ChangeIndex()
        {
            if (!previousTube.MultiDirection) return PathIndex;
            offsetIndex = GetOffSetIndex();
            Debug.Log($"{offsetIndex} + {connectedFarm.ConnectedTubes.Count - 1}");
            //if (offsetIndex >= (previousTube.Offset.Length - 1))
            //{
            //    PathIndex = 0;
            //    return PathIndex;
            //}
            //PathIndex = (connectedFarm.ConnectedTubes.Count - 1) - offsetIndex;
            PathIndex = offsetIndex;
            return PathIndex;
        }

        private int GetOffSetIndex()
        {
            RotateArray(offset);
            previousTube.RotateArray(previousTube.Offset);
            for (int j = 0; j < offset.Length; j++)
            {
                for (int i = 0; i < previousTube.Offset.Length; i++)
                {
                    Vector3 prevPos = previousTube.CorrectPosWithTransform(previousTube.Offset[i]);
                    Vector3 thisPos = CorrectPosWithTransform(offset[j]);

                    if (prevPos == thisPos)
                    {
                        similarIndex = j;
                        return connectedFarm.GetPath(prevPos);
                    }
                }
            }
            Debug.Log("How the fuck do they even connect then");
            return 0;
        }

        public void RotateArray<T>(T[] array, int customRotation = -1)
        {
            int swapIncrement = (int)(customRotation < 0 ? transform.rotation.eulerAngles.y / 90 : customRotation / 90);
            for (int i = 0; i < array.Length; i++)
            {
                int newIndex = i + swapIncrement;
                if (newIndex >= array.Length)
                {
                    newIndex = array.Length - swapIncrement;
                }
                array.Swap(i, newIndex);
                Debug.Log($"{i} is now {newIndex}");
            }
        }

        private void ConnectTubes()
        {
            if (connectedFarm == null) return;

            int pathIndex = previousTube == null ? 0 : ChangeIndex();
            if (multiDirection) goto MultiConnect;
            //Debug.Log($"{pathIndex} {  connectedFarm.ConnectedTubes.Count} ");
            for (int i = 0; i < positions.Length; i++)
            {
                Vector3 pos = CorrectPosWithRotation(positions[i]);
                connectedFarm.ConnectedTubes[pathIndex].Points.Add(pos);
                if (!connectedFarm.OffsetToPath.ContainsKey(pos))
                {
                    connectedFarm.OffsetToPath.Add(pos, pathIndex);
                }
            }
            return;

        MultiConnect:
            {
                int previousCount = connectedFarm.ConnectedTubes.Count - 1;
                for (int j = 0; j < Paths.Length - 1; j++)
                {
                    //First add all the new paths with a copy of the current path
                    Path copyPath = new Path();
                    copyPath.Points = new List<Vector3>(connectedFarm.ConnectedTubes[pathIndex].Points);
                    //connectedFarm.ConnectedTubes[0];
                    connectedFarm.ConnectedTubes.Add(copyPath);
                }

                ////Separated so that the copy will be the same for every new path
                RotateArray(Paths);
                int rotation = (similarIndex * 90) + (int)transform.rotation.y;
                for (int i = 0; i < Paths[0].Points.Count; i++)
                {
                    //Then add new points to the new paths
                    Debug.Log($"Adding at path: {pathIndex} from: {similarIndex}.{i}");
                    Vector3 pos = CorrectPosWithRotation(Paths[0].Points[i], rotation);
                    connectedFarm.ConnectedTubes[pathIndex].Points.Add(pos);
                    if (!connectedFarm.OffsetToPath.ContainsKey(pos))
                    {
                        connectedFarm.OffsetToPath.Add(pos, pathIndex);
                    }
                }

                for (int j = 1, multiIndex = previousCount + 1; j < Paths.Length; j++, multiIndex++)
                {
                    for (int i = 0; i < Paths[j].Points.Count; i++)
                    {
                        //Then add new points to the new paths
                        Debug.Log($"Adding at path: {multiIndex} from: {j}.{i}");
                        Vector3 pos = CorrectPosWithRotation(Paths[j].Points[i], rotation);

                        connectedFarm.ConnectedTubes[multiIndex].Points.Add(pos);
                        if (!connectedFarm.OffsetToPath.ContainsKey(pos))
                        {
                            connectedFarm.OffsetToPath.Add(pos, multiIndex);
                        }
                    }
                }
            }

            return;
        }

        //TODO: Just copy the connect and reverse the logic you bitch
        [Button]
        private void DisconnectTubes()
        {
            if (connectedFarm == null) return;

            int pathIndex = previousTube == null ? 0 : ChangeIndex();
            if (multiDirection) goto MultiConnect;
            for (int i = 0; i < positions.Length; i++)
            {
                Vector3 pos = CorrectPosWithRotation(positions[i]);
                connectedFarm.ConnectedTubes[pathIndex].Points.Remove(pos);
                if (connectedFarm.OffsetToPath.ContainsKey(pos))
                {
                    connectedFarm.OffsetToPath.Remove(pos);
                }
            }
            return;

        MultiConnect:
            {
                for (int j = 0; j < Paths.Length; j++)
                {
                    for (int i = 0; i < Paths[j].Points.Count; i++)
                    {
                        Vector3 pos = Paths[j].Points[i];
                        int index = connectedFarm.GetPath(pos);
                        connectedFarm.ConnectedTubes[index].Points.Remove(pos);
                        if (connectedFarm.OffsetToPath.ContainsKey(pos))
                        {
                            connectedFarm.OffsetToPath.Remove(pos);
                        }
                    }
                }
            }

            return;
        }

        private void OnGridChange()
        {
            adjustingTube = true;
            DisconnectTubes();
            connectedFarm = null;

            for (int i = 0; i < offset.Length; i++)
            {
                FarmStationFunction tempConnectedFarm = CheckForConnections(offset[i]);
                if (tempConnectedFarm != null)
                {
                    connectedFarm = tempConnectedFarm;
                    break;
                }
            }

            if (connectedFarm)
            {
                ConnectTubes();
            }
            adjustingTube = false;
        }

        [Button]
        private FarmStationFunction CheckForConnections(Vector3 offset)
        {
            Collider[] cols = Physics.OverlapSphere(CorrectPosWithRotation(offset), radius, layer);
            FarmStationFunction connectedFarm = null;
            if (cols.Length > 0)
            {
                Collider otherCol = GetOtherCollider(cols);
                if (!otherCol) return connectedFarm;
                connectedFarm = otherCol.transform.parent.gameObject.GetComponent<FarmStationFunction>();
                if (!connectedFarm)
                {
                    previousTube = otherCol.transform.parent.gameObject.GetComponent<TubeDirection>();
                    PathIndex = previousTube.PathIndex;
                    connectedFarm = previousTube.ConnectedFarm;
                }
                else
                {
                    PathIndex = 0;
                }
            }
            return connectedFarm;
        }

        public Vector3 CorrectPosWithRotation(Vector3 offset)
        {
            Vector3 pos = transform.rotation * (offset);
            pos += transform.position;
            return pos;
        }

        public Vector3 CorrectPosWithRotation(Vector3 offset, int rotation)
        {
            Quaternion rot = Quaternion.Euler(0, rotation, 0);
            Vector3 pos = rot * (offset);
            pos += transform.position;
            return pos;
        }

        public Vector3 CorrectPosWithTransform(Vector3 offset)
        {
            return transform.position + (offset);
        }

        private Collider GetOtherCollider(Collider[] cols)
        {
            Collider col = null;
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].transform.parent != transform)
                    return cols[i];
            }
            return col;
        }

        public void OnReturnObject()
        {
            DisconnectTubes();

            gameObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            DrawOverlapSphere();

            Gizmos.color = Color.cyan;
            if (multiDirection) goto MultiConnect;

            if (positions != null && positions.Length > 0)
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    Gizmos.DrawSphere(CorrectPosWithTransform(positions[i]), 0.1f);
                }
                return;
            }
        MultiConnect:
            {
                if (Paths != null && Paths.Length > 0)
                {
                    for (int j = 0; j < Paths.Length; j++)
                    {
                        if (Paths[j] == null || Paths[j].Points == null) continue;
                        for (int i = 0; i < Paths[j].Points.Count; i++)
                        {
                            Gizmos.DrawSphere(CorrectPosWithTransform(Paths[j].Points[i]), 0.1f);
                        }
                    }
                    return;
                }
            }
        }

        private void DrawRay()
        {
            // Gizmos.DrawLine(transform.position + offset, transform.position + offset + (rayDirection * rayDistance));
        }

        private void DrawOverlapSphere()
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < offset.Length; i++)
            {
                Gizmos.DrawWireSphere(CorrectPosWithRotation(offset[i]), radius);
            }
        }

        public override void OnEventRaised(Void value)
        {
        }
    }
}