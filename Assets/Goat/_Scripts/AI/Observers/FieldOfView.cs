using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Goat.Grid.Interactions;
using System.Linq;
using Goat.Storage;
using Goat.Events;
using Sirenix.OdinInspector;
using UnityAtoms.BaseAtoms;
using DG.Tweening;
using System;
using Goat.AI.Satisfaction;

namespace Goat.AI
{
    public class FieldOfView : MonoBehaviour
    {
        private const int MAX_OBSTACLES = 10;
        private const int MAX_TILES = 20;

        [Header("How fast a customer uses its FOV a second")]
        [SerializeField] private float viewingSpeed = 10;

        [SerializeField] private float viewRadius;
        [Range(0, 360)]
        [SerializeField] private float viewAngle;

        [SerializeField] private LayerMask tilesMask;
        [SerializeField] private LayerMask storageMask;

        [SerializeField] private LayerMask obstacleMask;
        [SerializeField] private LayerMask debugObstacles;

        [SerializeField] private List<Transform> visibleTargets = new List<Transform>();
        [SerializeField] private Transform[] visibleTargetsArray = new Transform[0];

        [SerializeField] private float meshResolution;
        [SerializeField] private int edgeResolveIterations;
        [SerializeField] private float edgeDstThreshold;

        [SerializeField] private float maskCutawayDst = .1f;

        private Mesh viewMesh;

        [SerializeField] private Customer customer;
        [Title("Events")]
        [SerializeField] private BeautyChecker beautyChecker;
        //[HideInInspector] StorageInteractable targetStorage, impulseStorage;
        private Sequence findTargetSequence;
        private WaitForSeconds waitDelay;
        private Collider[] tiles;
        [SerializeField] private RaycastHit[] obstaclesHits;

        private Collider currentTarget;
        [SerializeField] private Collider[] targetsInViewRadius;
        public float ViewAngle => viewAngle;

        public float ViewRadius => viewRadius;

        private void Awake()
        {
            viewMesh = new Mesh();
            viewMesh.name = "View Mesh";
            MeshFilter viewMeshFilter = gameObject.AddComponent<MeshFilter>();
            MeshRenderer viewMeshRenderer = gameObject.AddComponent<MeshRenderer>();
            viewMeshFilter.mesh = viewMesh;

            customer = GetComponentInParent<Customer>();

            findTargetSequence = DOTween.Sequence();
            findTargetSequence.SetLoops(-1);
            findTargetSequence.AppendCallback(FindVisibleTargets);
            findTargetSequence.AppendInterval(3);
            tiles = new Collider[MAX_TILES];
            obstaclesHits = new RaycastHit[MAX_OBSTACLES];
            waitDelay = new WaitForSeconds(2);
            //StartCoroutine(FindTargetsWithDelay());
        }

        private IEnumerator FindTargetsWithDelay()
        {
            while (true)
            {
                //if (customer.targetStorage == null && customer.enteredStore)
                //{
                FindVisibleTargets();
                Debug.Log("CALLED");

                //Debug.LogFormat("Looking with FOV {0}", customer.targetStorage == null);
                //}
                yield return waitDelay;
            }
        }

        //private void Update()
        //{
        //    if (Time.frameCount % 144 == 0)
        //    {
        //        FindVisibleTargets();
        //    }
        //}

        //private float timeSinceLastCalled;

        //private float delay = 0.5f;

        //private void Update()
        //{
        //    timeSinceLastCalled += Time.deltaTime;
        //    if (timeSinceLastCalled > delay)
        //    {
        //        FindVisibleTargets();
        //        timeSinceLastCalled = 0f;
        //    }
        //}

        //private void FixedUpdate()
        //{
        //    FindVisibleTargets();
        //    //DrawFieldOfView();
        //}

        /// <summary>
        /// Find targets based on layer.
        /// </summary>
        private void FindVisibleTargets()
        {
            // Clear target list to avoid duplicates.
            // Find targets in range.
            FindVisibleStorages();
            FindVisibleTiles();

            //  FindVisibleTiles();

            // order list by target distance form customer (and turn into array for faster alocation)
        }

        private void FindVisibleTiles()
        {
            if (Physics.OverlapSphereNonAlloc(transform.position, viewRadius, tiles, tilesMask) <= 0) return;

            for (int i = 0; i < tiles.Length; i++)
            {
                currentTarget = tiles[i];
                if (!currentTarget) continue;

                Vector3 dirToTarget = (currentTarget.bounds.center - transform.position).normalized;
                // Only check targets within viewing angle.
                if (!(Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)) continue;

                if (TargetObstructed(currentTarget, dirToTarget)) continue;

                beautyChecker.Positions.Add(currentTarget.gameObject.transform.position);
                //  tileTargetFoundEvent.Raise(currentTarget.gameObject.transform.position);
            }
        }

        private void FindVisibleStorages()
        {
            visibleTargets.Clear();

            targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, storageMask);

            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                currentTarget = targetsInViewRadius[i];
                Vector3 dirToTarget = (currentTarget.bounds.center - transform.position).normalized;
                // Only check targets within viewing angle.
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    if (TargetObstructed(currentTarget, dirToTarget)) continue;

                    CheckForStorageTarget(currentTarget);
                }
            }

            visibleTargetsArray = visibleTargets.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).ToArray();
            if (customer.ItemsToGet.ItemsInInventory > 0 && ContainsGroceries(out StorageInteractable targetStorage))
            {
                Debug.Log(targetStorage);
                customer.TargetStorage = targetStorage;
                customer.TargetDestination = targetStorage.transform.position;
                //Debug.Log("Found target to get item from!");
            }
        }

        public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        private bool TargetObstructed(Collider targetInView, Vector3 dirToTarget)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetInView.bounds.center);
            // Debug.DrawRay(transform.position, dirToTarget * 100, Color.magenta);
            //  return Physics.OverlapSphereNonAlloc(targetInView.bounds.center, 0.1f, obstacles, obstacleMask) > 0;
            //return Physics.OverlapSphere(targetInView.transform.position, 0.5f, obstacleMask).Length > 0;
            return (Physics.RaycastNonAlloc(transform.position, dirToTarget, obstaclesHits, distanceToTarget, obstacleMask)) > 0;
            //  return Physics.Linecast(transform.position, targetInView.transform.position, obstacleMask);
            //RaycastNonAlloc(Vector3 origin, Vector3 direction, RaycastHit[] results, float maxDistance, int layerMask);
        }

        private void CheckForStorageTarget(Collider[] targetsInViewRadius, int i, Vector3 dirToTarget)
        {
            if (targetsInViewRadius[i].tag == "Storage")
            {
                string tempStorageName = targetsInViewRadius[i].GetComponentInParent<StorageInteractable>().gameObject.name;
                float distanceToTarget = Vector3.Distance(transform.position, targetsInViewRadius[i].bounds.center);
                Debug.DrawRay(transform.position, dirToTarget * distanceToTarget, Color.red);

                // Check if Raycast hits target or if there is another target or obstacle blocking it.
                if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask) &&
                    Physics.Raycast(transform.position, dirToTarget, out RaycastHit hit, distanceToTarget, storageMask))
                {
                    if (hit.transform.tag.Equals("Storage"))
                        if (hit.transform.GetComponentInParent<StorageInteractable>().gameObject.name == tempStorageName)
                            visibleTargets.Add(targetsInViewRadius[i].transform);
                }
            }
        }

        private void CheckForStorageTarget(Collider target)
        {
            //if (customer.ItemsToGet.ItemsInInventory > 0 && ContainsGroceries(out StorageInteractable targetStorage))
            //{
            if (target.CompareTag("Storage"))
            {
                visibleTargets.Add(target.transform);
            }
            // }
        }

        //private void OnDrawGizmos()
        //{
        //    for (int i = 0; i < targetsInViewRadius.Length; i++)
        //    {
        //        if (!targetsInViewRadius[i]) continue;
        //        Vector3 dirToTarget = (targetsInViewRadius[i].bounds.center - transform.position).normalized;
        //        if (!(Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)) continue;

        //        Gizmos.color = TargetObstructed(targetsInViewRadius[i], dirToTarget) ? Color.red : Color.green;
        //        // Only check targets within viewing angle.
        //        Vector3 pos = targetsInViewRadius[i].bounds.center;
        //        Gizmos.DrawSphere(pos, 0.1f);
        //    }
        //}

        /// <summary>
        /// Checks if a visible target contains any of the items the customer is searching for.
        /// </summary>
        /// <param name="target"> Out parameter of type StorageInteractable</param>
        /// <returns></returns>
        private bool ContainsGroceries(out StorageInteractable target)
        {
            for (int i = 0; i < visibleTargetsArray.Length; i++)
            {
                StorageInteractable tempStorage = visibleTargetsArray[i].GetComponentInParent<StorageInteractable>();
                for (int j = 0; j < tempStorage.Inventory.Items.Count; j++)
                {
                    Resource tempResource = tempStorage.Inventory.Items.ElementAt(j).Key;
                    if (customer.ItemsToGet.Contains(tempResource))
                    {
                        target = tempStorage;
                        return true;
                    }
                }
            }
            target = null;
            return false;
        }

        //bool ImpulsePurchase()
        //{
        //    bool buy = false;

        //    return buy;
        //}

        #region MeshRenderer for Field of View

        private void DrawFieldOfView()
        {
            int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
            float stepAngleSize = viewAngle / stepCount;
            List<Vector3> viewPoints = new List<Vector3>();
            ViewCastInfo oldViewCast = new ViewCastInfo();
            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
                ViewCastInfo newViewCast = ViewCast(angle);

                if (i > 0)
                {
                    bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                    if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                    {
                        EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                        if (edge.pointA != Vector3.zero)
                            viewPoints.Add(edge.pointA);
                        if (edge.pointB != Vector3.zero)
                            viewPoints.Add(edge.pointB);
                    }
                }

                viewPoints.Add(newViewCast.point);
                oldViewCast = newViewCast;
            }

            int vertexCount = viewPoints.Count + 1;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector3.zero;
            for (int i = 0; i < vertexCount - 1; i++)
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]) + Vector3.forward * maskCutawayDst;

                if (i < vertexCount - 2)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }

            viewMesh.Clear();

            viewMesh.vertices = vertices;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals();
        }

        private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
        {
            float minAngle = minViewCast.angle;
            float maxAngle = maxViewCast.angle;
            Vector3 minPoint = Vector3.zero;
            Vector3 maxPoint = Vector3.zero;

            for (int i = 0; i < edgeResolveIterations; i++)
            {
                float angle = (minAngle + maxAngle) / 2;
                ViewCastInfo newViewCast = ViewCast(angle);

                bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
                {
                    minAngle = angle;
                    minPoint = newViewCast.point;
                }
                else
                {
                    maxAngle = angle;
                    maxPoint = newViewCast.point;
                }
            }

            return new EdgeInfo(minPoint, maxPoint);
        }

        private ViewCastInfo ViewCast(float globalAngle)
        {
            Vector3 dir = DirFromAngle(globalAngle, true);
            RaycastHit hit;

            if (Physics.Raycast(transform.position, dir, out hit, viewRadius, debugObstacles))
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
            else
                return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }

        public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
                angleInDegrees += transform.eulerAngles.y;

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        public struct ViewCastInfo
        {
            public bool hit;
            public Vector3 point;
            public float dst;
            public float angle;

            public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
            {
                hit = _hit;
                point = _point;
                dst = _dst;
                angle = _angle;
            }
        }

        public struct EdgeInfo
        {
            public Vector3 pointA;
            public Vector3 pointB;

            public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
            {
                pointA = _pointA;
                pointB = _pointB;
            }
        }

        #endregion MeshRenderer for Field of View
    }
}