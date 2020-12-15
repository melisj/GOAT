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
        [SerializeField] private Vector3[] offset;
        [SerializeField] private float radius = 0.2f;

        private TubeDirection previousTube;
        [SerializeField] private TubeDirection[] connectedTubes;

        public int PoolKey { get; set; }
        public ObjectInstance ObjInstance { get; set; }
        public FarmStationFunction ConnectedFarm { get => connectedFarm; set => connectedFarm = value; }
        public Vector3[] Offset => offset;

        public TubeDirection[] ConnectedTubes => connectedTubes;

        public Path[] Paths { get => paths; set => paths = value; }
        public Path Path { get => path; set => path = value; }

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

            if (index < 0) index = 0;
            return paths[index];
        }

        public void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            OnGridChange();
            connectedFarm = null;
            ObjInstance = objectInstance;
            PoolKey = poolKey;
        }

        public bool HasConnection()
        {
            int connections = 0;
            for (int i = 0; i < connectedTubes.Length; i++)
            {
                if (connectedTubes[i] != null)
                    connections++;
            }
            return (connections > 0);
        }

        public int NewRotation(int index)
        {
            return (index * 90) + (int)transform.rotation.y;
        }

        private void OnGridChange()
        {
            connectedTubes = new TubeDirection[offset.Length];

            for (int i = 0; i < offset.Length; i++)
            {
                FarmStationFunction tempConnectedFarm = CheckForConnectionsMulti(offset[i], i);
                if (tempConnectedFarm != null)
                {
                    connectedFarm = tempConnectedFarm;
                }
            }
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

                    connectedFarm = otherCol.transform.parent.gameObject.GetComponent<FarmStationFunction>();
                    if (connectedFarm)
                    {//At the first pipe
                        FlowSeeker seeker = connectedFarm.GetComponent<FlowSeeker>();
                        if (seeker.CollidedTubes.Count <= 0)
                        {
                            seeker.CollidedTubes.Add(this);
                        }
                    }
                    previousTube = otherCol.transform.parent.gameObject.GetComponent<TubeDirection>();

                    if (previousTube)
                    {
                        connectedTubes[index] = (previousTube);
                    }

                    if (!connectedFarm)
                    {
                        connectedFarm = previousTube.ConnectedFarm;
                    }
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

        private List<Collider> GetOtherColliders(Collider[] cols)
        {
            List<Collider> colList = new List<Collider>();
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].transform.parent != transform)
                {
                    colList.Add(cols[i]);
                }
            }
            return colList;
        }

        public void OnReturnObject()
        {
            SubcribedEvent.Raise();
            gameObject.SetActive(false);
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