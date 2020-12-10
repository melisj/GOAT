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
        [SerializeField, HideIf("multiDirection")] private Path path;
        [SerializeField, ShowIf("multiDirection")] private Path[] paths;
        [SerializeField] private LayerMask layer;
        //[SerializeField] private Vector3 rayDirection;
        //[SerializeField] private float rayDistance;
        [SerializeField] private Vector3[] offset;
        [SerializeField] private float radius = 0.2f;
        private int similarIndex;
        private List<int> similarIndexes;

        private int offsetIndex;
        private TubeDirection previousTube;
        [SerializeField] private TubeDirection[] connectedTubes;

        private bool adjustingTube;
        private int previousCount;
        private bool connected;
        public int PathIndex { get; set; }
        public int PoolKey { get; set; }
        public ObjectInstance ObjInstance { get; set; }
        public FarmStationFunction ConnectedFarm { get => connectedFarm; set => connectedFarm = value; }
        public Vector3[] Offset => offset;

        public bool MultiDirection => multiDirection;

        public TubeDirection[] ConnectedTubes => connectedTubes;

        public Path[] Paths => paths;

        public int GetPathCount()
        {
            if (paths != null)
            {
                int amountActivate = 0;
                for (int i = 0; i < connectedTubes.Length; i++)
                {
                    if (connectedTubes[i] != null)
                        amountActivate++;
                }
                return amountActivate - 2;
            }

            return -1;
        }

        public Path GetPath(int index)
        {
            if (!multiDirection)
                return path;

            //      RotateArray(Paths);
            if (index < 0) index = 0;
            return paths[index];
        }

        public void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            OnGridChange();
            connectedFarm = null;
            connected = false;
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
                        //This is fine till you have multiple connections leading to multiple connections
                        similarIndex = j;
                        return connectedFarm.GetPath(prevPos);
                    }
                }
            }
            Debug.Log("How the fuck do they even connect then");
            return 0;
        }

        private List<int> GetOffSetIndexes()
        {
            List<int> offsetIndexes = new List<int>();
            similarIndexes = new List<int>();
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
                        similarIndexes.Add(j);
                        offsetIndexes.Add(connectedFarm.GetPath(prevPos));
                    }
                }
            }

            return offsetIndexes;
        }

        public void RotateArray<T>(T[] array, int customRotation = -1)
        {
            int swapIncrement = (int)(customRotation < 0 ? (transform.rotation.eulerAngles.y) / 90 : (customRotation) / 90);
            for (int i = 0; i < array.Length; i++)
            {
                int newIndex = i + swapIncrement;
                if (newIndex >= array.Length)
                {
                    newIndex = swapIncrement - array.Length;
                }
                array.Swap(i, newIndex);
            }
        }

        private void InterConnectTubes()
        {
            if (connectedFarm == null) return;

            if (previousTube == null)
            {
                ConnectTubes();
                return;
            }

            for (int preT = 0; preT < connectedTubes.Length; preT++)
            {
                previousTube = connectedTubes[preT];

                int pathIndex = ChangeIndex();

                if (multiDirection) goto MultiConnect;

                for (int i = 0; i < path.Points.Count; i++)
                {
                    Vector3 pos = CorrectPosWithRotation(path.Points[i]);
                    connectedFarm.ConnectedTubes[pathIndex].Points.Add(pos);
                    if (!connectedFarm.OffsetToPath.ContainsKey(pos))
                    {
                        connectedFarm.OffsetToPath.Add(pos, pathIndex);
                    }
                }

            MultiConnect:
                {
                    ConnectMultiTubes(pathIndex);
                }
            }
            connected = true;
        }

        private void ConnectTubes()
        {
            if (connectedFarm == null) return;

            int pathIndex = previousTube == null ? 0 : ChangeIndex();
            if (multiDirection) goto MultiConnect;
            //Debug.Log($"{pathIndex} {  connectedFarm.ConnectedTubes.Count} ");
            for (int i = 0; i < path.Points.Count; i++)
            {
                Vector3 pos = CorrectPosWithRotation(path.Points[i]);
                connectedFarm.ConnectedTubes[pathIndex].Points.Add(pos);
                if (!connectedFarm.OffsetToPath.ContainsKey(pos))
                {
                    connectedFarm.OffsetToPath.Add(pos, pathIndex);
                }
            }
            connected = true;
            return;

        MultiConnect:
            {
                ConnectMultiTubes(pathIndex);
                connected = true;
            }
            return;
        }

        private void ConnectMultiTubes(int pathIndex)
        {
            previousCount = connectedFarm.ConnectedTubes.Count - 1;
            for (int j = 0; j < paths.Length - 1; j++)
            {
                //First add all the new paths with a copy of the current path
                Path copyPath = new Path();
                copyPath.Points = new List<Vector3>(connectedFarm.ConnectedTubes[pathIndex].Points);
                //connectedFarm.ConnectedTubes[0];
                connectedFarm.ConnectedTubes.Add(copyPath);
            }

            ////Separated so that the copy will be the same for every new path
            RotateArray(paths);
            int rotation = NewRotation(similarIndex);
            for (int i = 0; i < paths[0].Points.Count; i++)
            {
                //Then add new points to the new paths
                Debug.Log($"Adding at path: {pathIndex} from: {similarIndex}.{i}");
                Vector3 pos = CorrectPosWithRotation(paths[0].Points[i], rotation);
                connectedFarm.ConnectedTubes[pathIndex].Points.Add(pos);
                if (!connectedFarm.OffsetToPath.ContainsKey(pos))
                {
                    connectedFarm.OffsetToPath.Add(pos, pathIndex);
                }
            }

            for (int j = 1, multiIndex = previousCount + 1; j < paths.Length; j++, multiIndex++)
            {
                for (int i = 0; i < paths[j].Points.Count; i++)
                {
                    //Then add new points to the new paths
                    Debug.Log($"Adding at path: {multiIndex} from: {j}.{i}");
                    Vector3 pos = CorrectPosWithRotation(paths[j].Points[i], rotation);

                    connectedFarm.ConnectedTubes[multiIndex].Points.Add(pos);
                    if (!connectedFarm.OffsetToPath.ContainsKey(pos))
                    {
                        connectedFarm.OffsetToPath.Add(pos, multiIndex);
                    }
                }
            }
        }

        public int NewRotation(int index)
        {
            return (index * 90) + (int)transform.rotation.y;
        }

        //TODO: Just copy the connect and reverse the logic you bitch
        [Button]
        private void DisconnectTubes()
        {
            if (connectedFarm == null) return;

            int pathIndex = previousTube == null ? 0 : ChangeIndex();
            if (multiDirection) goto MultiConnect;
            //Debug.Log($"{pathIndex} {  connectedFarm.ConnectedTubes.Count} ");
            for (int i = 0; i < path.Points.Count; i++)
            {
                Vector3 pos = CorrectPosWithRotation(path.Points[i]);
                connectedFarm.ConnectedTubes[pathIndex].Points.Remove(pos);
                if (connectedFarm.OffsetToPath.ContainsKey(pos))
                {
                    connectedFarm.OffsetToPath.Remove(pos);
                }
            }
            return;

        MultiConnect:
            {
                DisconnectMultiTubes(pathIndex);
            }
            return;
        }

        private void DisconnectMultiTubes(int pathIndex)
        {
            if (previousCount >= connectedFarm.ConnectedTubes.Count) return;
            //for (int j = 0; j < Paths.Length - 1; j++)
            //{
            //    //First add all the new paths with a copy of the current path
            //    Path copyPath = new Path();
            //    copyPath.Points = new List<Vector3>(connectedFarm.ConnectedTubes[pathIndex].Points);
            //    //connectedFarm.ConnectedTubes[0];
            //    connectedFarm.ConnectedTubes.Add(copyPath);
            //}

            ////Separated so that the copy will be the same for every new path
            RotateArray(paths);
            int rotation = (similarIndex * 90) + (int)transform.rotation.y;
            for (int i = 0; i < paths[0].Points.Count; i++)
            {
                //Then add new points to the new paths
                Debug.Log($"Removing at path: {pathIndex} from: {similarIndex}.{i}");
                Vector3 pos = CorrectPosWithRotation(paths[0].Points[i], rotation);
                connectedFarm.ConnectedTubes[pathIndex].Points.Remove(pos);
                if (connectedFarm.OffsetToPath.ContainsKey(pos))
                {
                    connectedFarm.OffsetToPath.Remove(pos);
                }
            }

            for (int j = 1, multiIndex = previousCount + 1; j < paths.Length; j++, multiIndex++)
            {
                for (int i = 0; i < paths[j].Points.Count; i++)
                {
                    //Then add new points to the new paths
                    Debug.Log($"Removing at path: {multiIndex} from: {j}.{i}");
                    Vector3 pos = CorrectPosWithRotation(paths[j].Points[i], rotation);

                    if (multiIndex < connectedFarm.ConnectedTubes.Count)
                        connectedFarm.ConnectedTubes[multiIndex].Points.Remove(pos);
                    // connectedFarm.ConnectedTubes.RemoveAt(multiIndex);
                    if (connectedFarm.OffsetToPath.ContainsKey(pos))
                    {
                        connectedFarm.OffsetToPath.Remove(pos);
                    }
                }
            }
        }

        private void OnGridChange()
        {
            adjustingTube = true;
            //   DisconnectTubes();
            //  if (previousTubes.Length <= 0)
            //   {
            connectedTubes = new TubeDirection[offset.Length];
            // }
            //   if (!connectedFarm)
            //    {
            for (int i = 0; i < offset.Length; i++)
            {
                FarmStationFunction tempConnectedFarm = CheckForConnectionsMulti(offset[i], i);
                if (tempConnectedFarm != null)
                {
                    connectedFarm = tempConnectedFarm;
                }
            }
            //   }

            if (connectedFarm && !connected)
            {
                //ConnectTubes();
                //InterConnectTubes();
            }
            adjustingTube = false;
        }

        [Button]
        private FarmStationFunction CheckForConnections(Vector3 offset, int index)
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
                    Debug.Log(previousTube);
                    connectedTubes[index] = previousTube;
                    Debug.Log(connectedTubes[index]);

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

        private FarmStationFunction CheckForConnectionsMulti(Vector3 offset, int index)
        {
            Collider[] cols = Physics.OverlapSphere(CorrectPosWithRotation(offset), radius, layer);
            FarmStationFunction connectedFarm = null;
            if (cols.Length > 0)
            {
                List<Collider> otherCols = GetOtherColliders(cols);
                for (int i = 0; i < otherCols.Count; i++)
                {
                    Collider otherCol = otherCols[i];

                    // if (!otherCol) return connectedFarm;
                    connectedFarm = otherCol.transform.parent.gameObject.GetComponent<FarmStationFunction>();
                    if (connectedFarm)
                    {//At the first pipe
                        FlowSeeker seeker = connectedFarm.GetComponent<FlowSeeker>();
                        if (seeker.CollidedTubes.Count <= 0)
                        {
                            seeker.CollidedTubes.Add(this);
                        }
                    }
                    //if (!connectedFarm)
                    //{
                    previousTube = otherCol.transform.parent.gameObject.GetComponent<TubeDirection>();
                    if (previousTube)
                    {
                        connectedTubes[index] = (previousTube);
                    }
                    // PathIndex = previousTube.PathIndex;
                    if (!connectedFarm)
                    {
                        connectedFarm = previousTube.ConnectedFarm;
                    }
                    //}
                    //else
                    //{
                    //    PathIndex = 0;
                    //}
                }
            }
            return connectedFarm;
        }

        public bool CheckEnds(TubeDirection prevCall = null)
        {
            for (int i = 0; i < connectedTubes.Length; i++)
            {
                TubeDirection currentTube = connectedTubes[i];
                if (currentTube)
                {
                    if (prevCall)
                    {
                        if (currentTube == prevCall)
                            continue;
                    }

                    if (!currentTube.CompareTag("TubeEnd"))
                    {
                        if (!currentTube.CheckEnds(this))
                        {
                            connectedTubes[i] = null;
                            continue;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [Button]
        public void CheckForEnd()
        {
            List<int> ends = new List<int>();
            CheckEnds();
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

        private List<Collider> GetOtherColliders(Collider[] cols)
        {
            List<Collider> colList = new List<Collider>();
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].transform.parent != transform)
                {
                    colList.Add(cols[i]);
                }
                // return cols[i];
            }
            return colList;
        }

        public void OnReturnObject()
        {
            //     DisconnectTubes();
            SubcribedEvent.Raise();
            gameObject.SetActive(false);
        }

        [Button]
        private void RotatePathArray()
        {
            RotateArray(paths);
            paths.Rotate((int)((transform.rotation.eulerAngles.y) / 90));
        }

        [Button]
        private void RotatePoints(int index)
        {
            paths[index].Points.Rotate((int)((transform.rotation.eulerAngles.y) / 90));
        }

        private void OnDrawGizmos()
        {
            DrawOverlapSphere();
            Gizmos.color = Color.cyan;
            if (multiDirection) goto MultiConnect;

            if (path.Points != null && path.Points.Count > 0)
            {
                for (int i = 0; i < path.Points.Count; i++)
                {
                    Gizmos.DrawSphere(CorrectPosWithRotation(path.Points[i]), 0.1f);
                }
                return;
            }
        MultiConnect:
            {
                if (paths != null && paths.Length > 0)
                {
                    for (int j = 0; j < paths.Length; j++)
                    {
                        if (paths[j] == null || paths[j].Points == null) continue;
                        for (int i = 0; i < paths[j].Points.Count; i++)
                        {
                            Gizmos.DrawSphere(CorrectPosWithRotation(paths[j].Points[i]), 0.1f);
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
            for (int i = 0; i < offset.Length; i++)
            {
                if (connectedTubes != null && connectedTubes.Length > 0)
                {
                    Gizmos.color = connectedTubes[i] == null ? Color.red : Color.green;
                }

                Gizmos.DrawWireSphere(CorrectPosWithRotation(offset[i]), radius);
            }
        }

        protected override void InitOnEnable()
        {
            base.InitOnEnable();
            SubcribedEvent.Raise();
        }

        protected override void InitOnDisable()
        {
            base.InitOnDisable();
            SubcribedEvent.Raise();
        }

        public override void OnEventRaised(Void value)
        {
            OnGridChange();
        }
    }
}