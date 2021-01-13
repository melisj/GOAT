using Goat.Events;
using Goat.Pooling;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Goat.Farming
{
    public class TubeDirection : MonoBehaviour, IPoolObject
    {
        [SerializeField] private FarmStationFunction connectedFarm;
        [SerializeField] private GameObjectEvent onGridChange;
        [SerializeField] private VoidEvent onTubesConnected;
        [SerializeField, ShowIf("ExchangePoint")] private FarmNetworkData networkData;
        [SerializeField] private bool multiDirection;
        [SerializeField] private bool endPoint;
        [SerializeField] private bool isFarmStation;
        [SerializeField, ShowIf("multiDirection")] private int connectionAmount;
        [SerializeField, ShowIf("multiDirection")] private TubeDirection[] connectedMultiDirections;
        [SerializeField, ShowIf("multiDirection")] private int[] distanceTillNextDirection;
        [SerializeField] private LayerMask layer;
        [SerializeField] private Vector3[] offset;
        [SerializeField] private float radius = 0.2f;
        private TileAnimation tileAnimation;
        [SerializeField] private List<TubeDirection> connectedTubes;

        public bool ExchangePoint => multiDirection || endPoint || isFarmStation;
        public bool EndPoint => endPoint;
        public bool IsFarmStation => isFarmStation;

        public int PoolKey { get; set; }
        public ObjectInstance ObjInstance { get; set; }
        public FarmStationFunction ConnectedFarm { get => connectedFarm; set => connectedFarm = value; }
        public Vector3[] Offset => offset;

        public List<TubeDirection> ConnectedTubes => connectedTubes;
        public int ConnectionAmount => connectionAmount;
        public TubeDirection[] ConnectedMultiDirections { get => connectedMultiDirections; set => connectedMultiDirections = value; }
        public int[] DistanceTillNextDirection { get => distanceTillNextDirection; set => distanceTillNextDirection = value; }

        // <Dijkstra info>
        public int DistanceFromStart;
        public bool VisitedByAlgorithm;
        // </Dijkstra info>

        public void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            if (ExchangePoint)
                networkData.AddPipe(this);

            if (!tileAnimation)
                tileAnimation = GetComponent<TileAnimation>();
            tileAnimation.Prepare();
            tileAnimation.Create(SetConnections);
            connectedFarm = null;
            ObjInstance = objectInstance;
            PoolKey = poolKey;
        }

        public bool HasConnection()
        {
            int connections = 0;
            bool connectedToFarm = false;
            for (int i = 0; i < connectedTubes.Count; i++)
            {
                if (connectedTubes[i] != null)
                {
                    connections++;
                    if (connectedTubes[i].ConnectedFarm)
                        connectedToFarm = true;
                }
            }
            return (connections > 0 && connectedToFarm);
        }

        public int NewRotation(int index)
        {
            return (index * 90) + (int)transform.rotation.y;
        }

        private void SetConnections()
        {
            connectedTubes.Clear();
            for (int i = 0; i < offset.Length; i++)
            {
                CheckForConnection(offset[i]);
            }
            onGridChange.Raise(gameObject);
        }

        private void CheckForConnection(Vector3 offset)
        {
            // Check all directions for tubes
            Collider[] cols = Physics.OverlapSphere(CorrectPosWithRotation(offset), radius, layer);
            // Assign those tubes as references
            if (cols.Length > 0)
            {
                List<Collider> otherCols = GetOtherColliders(cols);
                for (int i = 0; i < otherCols.Count; i++)
                {
                    Collider otherCol = otherCols[i];
                    Debug.Log(otherCol.gameObject.name,  otherCol.gameObject);

                    TubeDirection tubeDir = otherCol.transform.parent.gameObject.GetComponent<TubeDirection>();
                    if (tubeDir) 
                    {
                        if (!ConnectedTubes.Contains(tubeDir))
                            connectedTubes.Add(tubeDir);

                        // Assign this tube to their references
                        if(!tubeDir.ConnectedTubes.Contains(this))
                            tubeDir.ConnectedTubes.Add(this);
                    }
                }
            }
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
            Debug.LogError("Returning");
            if(ExchangePoint)
                networkData.RemovePipe(this);

            for (int i = 0; i < connectedTubes.Count; i++)
            {
                connectedTubes[i].ConnectedTubes.Remove(this);
            }
            ConnectedTubes.Clear();

            tileAnimation.Destroy(() => 
            {
                onGridChange.Raise(gameObject);
                gameObject.SetActive(false);
            });
        }

        private void OnDestroy() 
        {
            if (ExchangePoint)
                networkData.RemovePipe(this);
        }

        private void OnDrawGizmos()
        {
            DrawOverlapSphere();
            Gizmos.color = Color.cyan;
            if (ExchangePoint) 
            { 
                Gizmos.DrawSphere(transform.position + transform.up * 1, 0.3f);

                for (int i = 0; i < ConnectedMultiDirections.Length; i++)
                {
                    if(ConnectedMultiDirections[i])
                        Gizmos.DrawLine(transform.position + transform.up * 1, ConnectedMultiDirections[i].transform.position + transform.up * 1);
                }
            }
        }

        private void DrawOverlapSphere()
        {
            for (int i = 0; i < connectedTubes.Count; i++)
            {
                if (connectedTubes != null && connectedTubes.Count > 0)
                {
                    Gizmos.color = connectedTubes[i] == null ? Color.red : Color.green;
                }

            }
            for (int i = 0; i < offset.Length; i++)
            {
                Gizmos.DrawWireSphere(CorrectPosWithRotation(offset[i]), radius);
            }
        }

        protected void OnEnable()
        {
            //onGridChange.RegisterSafe(this);
            Debug.Log("activate");
        }

        protected void OnDisable()
        {
            //onGridChange.UnregisterSafe(this);
            for (int i = 0; i < connectedTubes.Count; i++)
            {
                connectedTubes[i].ConnectedTubes.Remove(this);
            }
            Debug.Log("deactivate");
        }

        
    }
}