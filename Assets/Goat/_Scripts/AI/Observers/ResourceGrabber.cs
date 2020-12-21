using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Storage;
using Goat.Pooling;

namespace Goat.AI
{
    public class ResourceGrabber : MonoBehaviour
    {
        [SerializeField] private NPC npc;
        private LayerMask layerMask;

        private void Awake()
        {
            layerMask = LayerMask.GetMask("DroppedResource");
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == layerMask)
            {
                Resource resource = other.GetComponent<Resource>();
                if(resource != null)
                {
                    npc.Inventory.Add(resource, 1, out int amountAdded);
                    if(amountAdded > 0)
                    {
                        PoolManager.Instance.ReturnToPool(other.gameObject);
                    }
                }
            }
        }

    }
}

