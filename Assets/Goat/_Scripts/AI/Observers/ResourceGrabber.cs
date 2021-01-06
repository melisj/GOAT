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

        //private void Awake()
        //{
        //    layerMask = LayerMask.GetMask("ResourcePack");
        //}

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Collider entered" + other.name);
            Resource resource = other.GetComponent<ResourcePack>().Resource;
            if (resource != null)
            {
                npc.Inventory.Add(resource, 1, out int amountAdded);
                if (amountAdded > 0)
                {
                    Debug.LogFormat("picked up {0}", resource.name);
                    PoolManager.Instance.ReturnToPool(other.gameObject);
                }
            }
        }
    }
}

