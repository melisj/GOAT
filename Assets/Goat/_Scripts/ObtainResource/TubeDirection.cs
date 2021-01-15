using Goat.Events;
using Goat.Pooling;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Goat.Farming
{
    public class TubeDirection : MonoBehaviour, IPoolObject
    {
        [SerializeField, TabGroup("References"), Required] private GameObjectEvent onGridChange;
        [SerializeField, TabGroup("References"), Required, ShowIf("ExchangePoint")] private FarmNetworkData networkData;

        // <Connection Settings>
        [SerializeField, TabGroup("Connection")] private bool multiDirection;
        [SerializeField, TabGroup("Connection")] private bool endPoint;
        [SerializeField, TabGroup("Connection")] private bool isFarmStation;

        [SerializeField, TabGroup("Data"), ShowIf("multiDirection"), ReadOnly] private TubeDirection[] connectedMultiDirections;
        [SerializeField, TabGroup("Data"), ShowIf("multiDirection"), ReadOnly] private int[] distanceTillNextDirection;
        [SerializeField, TabGroup("Data"), ReadOnly] private List<TubeDirection> connectedTubes;

        [SerializeField, TabGroup("Connection")] private LayerMask layer;
        [SerializeField, TabGroup("Connection")] private Vector3[] offset;
        // <Connection Settings>

        [SerializeField, TabGroup("Debug")] private float radius = 0.2f;

        // <Dijkstra info>
        [TabGroup("Dijkstra")] public int DistanceFromStart;
        [TabGroup("Dijkstra")] public bool VisitedByAlgorithm;
        // </Dijkstra info>

        private TileAnimation tileAnimation;

        public bool ExchangePoint => multiDirection || endPoint || isFarmStation;
        public bool EndPoint => endPoint;
        public bool IsFarmStation => isFarmStation;

        public int PoolKey { get; set; }
        public ObjectInstance ObjInstance { get; set; }

        public List<TubeDirection> ConnectedTubes => connectedTubes;
        public int ConnectionAmount => offset.Length;
        public TubeDirection[] ConnectedMultiDirections { get => connectedMultiDirections; set => connectedMultiDirections = value; }
        public int[] DistanceTillNextDirection { get => distanceTillNextDirection; set => distanceTillNextDirection = value; }

        private void Connect()
        {
            connectedTubes.Clear();
            for (int i = 0; i < offset.Length; i++)
            {
                CheckForConnection(offset[i]);
            }
            onGridChange.Raise(gameObject);
        }

        private void Disconnect()
        {
            for (int i = 0; i < connectedTubes.Count; i++)
            {
                connectedTubes[i].ConnectedTubes.Remove(this);
            }
            ConnectedTubes.Clear();
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

                    TubeDirection tubeDir = otherCol.transform.parent.gameObject.GetComponent<TubeDirection>();
                    if (tubeDir) 
                    {
                        if (!ConnectedTubes.Contains(tubeDir))
                            connectedTubes.Add(tubeDir);

                        // Assign this tube to their references
                        if(!tubeDir.ConnectedTubes.Contains(this))
                            tubeDir.ConnectedTubes.Add(this);

                        break;
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

        public void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            if (ExchangePoint)
                networkData.AddPipe(this);

            if (!tileAnimation)
                tileAnimation = GetComponent<TileAnimation>();
            tileAnimation.Prepare();
            tileAnimation.Create(Connect);
            ObjInstance = objectInstance;
            PoolKey = poolKey;
        }

        public void OnReturnObject()
        {
            if(ExchangePoint)
                networkData.RemovePipe(this);

            Disconnect();

            tileAnimation.Destroy(() => 
            {
                gameObject.SetActive(false);
            });
        }

        private void OnDestroy() 
        {
            if (ExchangePoint)
                networkData.RemovePipe(this);

            Disconnect();
        }

        protected void OnEnable()
        {
        }

        protected void OnDisable()
        {
        }

        #region Gizmos 
        private void OnDrawGizmos()
        {
            Gizmos.color = VisitedByAlgorithm ? Color.green : Color.cyan;
            if (ExchangePoint)
            {
                Gizmos.DrawSphere(transform.position + transform.up, radius);

                for (int i = 0; i < ConnectedMultiDirections.Length; i++)
                {
                    if (ConnectedMultiDirections[i])
                        Gizmos.DrawLine(transform.position + transform.up, ConnectedMultiDirections[i].transform.position + transform.up);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            DrawOverlapSphere();
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

        #endregion
    }
}