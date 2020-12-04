using Goat.Grid.Interactions;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    [Title("OverlapSphere method")]
    [SerializeField] private Vector3 center;
    [SerializeField] private int size;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private bool repeatDetectionOverTime;
    [SerializeField, ShowIf("repeatDetectionOverTime")] private int intervalTime;
    [SerializeField] private bool updateDetectionOnIdleOnly;
    private Vector3 oldPos;
    private bool isMoving;
    private Collider latestCollider, previousCollider;

    private BaseInteractable previousInteractable;

    //public event EventHandler<Collider> OnColliderEnter;

    private void Awake()
    {
        if (repeatDetectionOverTime)
            InvokeRepeating("DetectNearest", 1, intervalTime);
    }

    private void Update()
    {
        isMoving = (transform.position != oldPos);
        oldPos = transform.position;

        if (!isMoving && updateDetectionOnIdleOnly && !latestCollider)
        {
            DetectNearest();
        }

        if (isMoving)
        {
            ResetUI();
        }
    }

    private void ResetUI()
    {
        if (previousInteractable)
        {
            previousInteractable.CloseUI();
            previousInteractable = null;
            latestCollider = null;
        }
    }

    private void DetectNearest()
    {
        latestCollider = GetNearest(DetectOverlap());

        if (latestCollider != null)
        {
            previousInteractable = latestCollider.GetComponentInParent<BaseInteractable>();
            if (previousInteractable.IsClickedOn)
            {
                previousInteractable.OpenUIFully();
            }
        }
        previousCollider = latestCollider;
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

    private Collider[] DetectOverlap()
    {
        return Physics.OverlapSphere(transform.position + center, size, layerMask);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = latestCollider == null ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position + center, size);
    }
}