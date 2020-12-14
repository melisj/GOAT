using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Goat.Grid.Interactions;
using System.Linq;

namespace Goat.AI
{
    public class FieldOfView : MonoBehaviour
    {
        [Header("How fast a customer uses its FOV a second")]
        [SerializeField] private float viewingSpeed = 10;

        [SerializeField] private float viewRadius;
        [Range(0, 360)]
        [SerializeField] private float viewAngle;

        [SerializeField] private LayerMask targetMask;
        [SerializeField] private LayerMask obstacleMask;
        [SerializeField] private LayerMask debugObstacles;

        private List<Transform> visibleTargets = new List<Transform>();
        private Transform[] visibleTargetsArray = new Transform[0];

        [SerializeField] private float meshResolution;
        [SerializeField] private int edgeResolveIterations;
        [SerializeField] private float edgeDstThreshold;

        [SerializeField] private float maskCutawayDst = .1f;

        Mesh viewMesh;

        [SerializeField] private Customer customer;
        //[HideInInspector] StorageInteractable targetStorage, impulseStorage;

        void Awake()
        {
            viewMesh = new Mesh();
            viewMesh.name = "View Mesh";
            MeshFilter viewMeshFilter = gameObject.AddComponent<MeshFilter>();
            MeshRenderer viewMeshRenderer = gameObject.AddComponent<MeshRenderer>();
            viewMeshFilter.mesh = viewMesh;

            customer = GetComponentInParent<Customer>();

            //StartCoroutine(FindTargetsWithDelay(viewingSpeed));
        }


        IEnumerator FindTargetsWithDelay(float delay)
        {
            while (true)
            {
                if(customer.targetStorage == null && customer.enteredStore)
                {
                    FindVisibleTargets();
                    //Debug.LogFormat("Looking with FOV {0}", customer.targetStorage == null);
                }
                yield return new WaitForSeconds(1 / delay);
            }
        }

        private void FixedUpdate()
        {
            FindVisibleTargets();
            //DrawFieldOfView();
        }

        /// <summary>
        /// Find targets based on layer.
        /// </summary>
        void FindVisibleTargets()
        {
            // Clear target list to avoid duplicates.
            visibleTargets.Clear();
            // Find targets in range.
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Vector3 dirToTarget = (targetsInViewRadius[i].bounds.center - transform.position).normalized;
                // Only check targets within viewing angle.
                if(Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    //targetsInRadius++;
                    if(targetsInViewRadius[i].tag == "Storage")
                    {
                        string tempStorageName = targetsInViewRadius[i].GetComponentInParent<StorageInteractable>().gameObject.name;
                        float distanceToTarget = Vector3.Distance(transform.position, targetsInViewRadius[i].bounds.center);
                        Debug.DrawRay(transform.position, dirToTarget * distanceToTarget, Color.red);

                        // Check if Raycast hits target or if there is another target or obstacle blocking it.
                        if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask) &&
                            Physics.Raycast(transform.position, dirToTarget, out RaycastHit hit, distanceToTarget, targetMask))
                        {
                            if(hit.transform.tag.Equals("Storage"))
                                if (hit.transform.GetComponentInParent<StorageInteractable>().gameObject.name == tempStorageName)
                                    visibleTargets.Add(targetsInViewRadius[i].transform);
                        }
                    }
                }
            }

            // order list by target distance form customer (and turn into array for faster alocation)
            visibleTargetsArray = visibleTargets.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).ToArray();
            if (customer.itemsToGet.Count > 0 && ContainsGroceries(out StorageInteractable targetStorage))
            {
                customer.targetStorage = targetStorage;
                customer.targetDestination = targetStorage.transform.position;
                //Debug.Log("Found target to get item from!");
            }
        }

        /// <summary>
        /// Checks if a visible target contains any of the items the customer is searching for.
        /// </summary>
        /// <param name="target"> Out parameter of type StorageInteractable</param>
        /// <returns></returns>
        bool ContainsGroceries(out StorageInteractable target)
        {
            for (int i = 0; i < visibleTargetsArray.Length; i++)
            {
                StorageInteractable tempStorage = visibleTargetsArray[i].GetComponentInParent<StorageInteractable>();
                for (int j = 0; j < tempStorage.GetItemCount; j++)
                {
                    if (customer.itemsToGet.ContainsKey(tempStorage.GetItems[j].Resource))
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

        void DrawFieldOfView()
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


        EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
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


        ViewCastInfo ViewCast(float globalAngle)
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

        #endregion
    }
}
