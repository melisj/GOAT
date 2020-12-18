using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class CollisionDetection : MonoBehaviour, ICollideDetect
{
    [Title("OverlapSphere method")]
    [SerializeField] private Vector3 center;

    [SerializeField] private int size;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private bool onlyNearest;
    [SerializeField, HideIf("onlyNearest")] private int maxAmountColliders;
    [SerializeField] private bool repeatDetectionOverTime;
    [SerializeField, ShowIf("repeatDetectionOverTime")] private float intervalTime;
    [SerializeField] private bool updateDetectionOnIdleOnly;
    private Vector3 oldPos;
    private bool isMoving;
    protected Collider latestCollider, previousCollider;
    protected Collider[] allColliders, allPreviousColliders;
    private Sequence detectSequence;
    public bool IsMoving { get => isMoving; }

    private void Awake()
    {
        if (repeatDetectionOverTime)
        {
            if (onlyNearest)
            {
                detectSequence = DOTween.Sequence();
                detectSequence.SetLoops(-1);
                detectSequence.AppendCallback(DetectNearest);
                detectSequence.AppendInterval(intervalTime);
            }
            else
            {
                detectSequence = DOTween.Sequence();
                detectSequence.SetLoops(-1);
                detectSequence.AppendCallback(DetectAll);
                detectSequence.AppendInterval(intervalTime);
            }
        }
    }

    //private void Update()
    //{
    //    if (updateDetectionOnIdleOnly)
    //    {
    //        isMoving = (transform.position != oldPos);
    //        oldPos = transform.position;

    //        if (!isMoving)
    //        {
    //            Detect();
    //        }
    //    }
    //    else
    //    {
    //        Detect();
    //    }
    //}

    private void Detect()
    {
        if (onlyNearest)
        {
            DetectNearest();
        }
        else
        {
            DetectAll();
        }
    }

    public abstract void OnExit();

    /// <summary>
    /// Use latest collider or all Colliders
    /// </summary>
    public abstract void OnEnter();

    private void DetectNearest()
    {
        latestCollider = GetNearest(DetectOverlap());

        if (latestCollider != null)
        {
            if (previousCollider != latestCollider)
            {
                OnExit();

                OnEnter();
            }
        }
        else
        {
            OnExit();
        }
        previousCollider = latestCollider;
    }

    private void DetectAll()
    {
        allColliders = DetectOverlapNonAlloc();
        if (allColliders.Length > 0)
        {
            OnExit();

            OnEnter();
        }
        else
        {
            OnExit();
        }

        if (allColliders != null)
        {
            allPreviousColliders = new Collider[allColliders.Length];
            allColliders.CopyTo(allPreviousColliders, 0);
        }
    }

    private Collider GetNearest(Collider[] colls)
    {
        float nearestDist = 9999;
        Collider nearestCollider = null;
        for (int i = 0; i < colls.Length; i++)
        {
            float currentDist = (transform.position - colls[i].transform.position).sqrMagnitude;
            if (currentDist < nearestDist)
            {
                nearestDist = currentDist;
                nearestCollider = colls[i];
            }
        }
        return nearestCollider;
    }

    private Collider[] DetectOverlapNonAlloc()
    {
        return Physics.OverlapSphere(transform.position + center, size, layerMask);
    }

    private Collider[] DetectOverlap()
    {
        return Physics.OverlapSphere(transform.position + center, size, layerMask);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = onlyNearest ? (latestCollider == null ? Color.red : Color.green) : allColliders != null && allColliders.Length > 0 ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position + center, size);
    }
}