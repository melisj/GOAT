using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.AI
{
    public class ResourceDetection : MonoBehaviour
    {
        [SerializeField] private float detectionRange = 3;
        [HideInInspector] public bool detected = false;
        [SerializeField] WarehouseWorker worker;
        [SerializeField] private LayerMask layerMask;

        //private void Awake()
        //{
        //    LayerMask.GetMask("ResourcePack");
        //}

        private void FixedUpdate()
        {
            if (worker.searching)
            {
                DetectResources();
            }
        }

        private void DetectResources()
        {
            detected = false;
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, layerMask);
            Vector3 resourcePosition = Vector3.zero;

            if (colliders != null && colliders.Length > 0)
            {
                detected = true;
                resourcePosition = colliders[0].transform.position;
                worker.targetDestination = resourcePosition;
            }
        }
    }
}

