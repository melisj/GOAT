using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Storage;
using Goat.Pooling;
using Sirenix.OdinInspector;
using Goat.Player;

namespace Goat.AI
{
    public class ResourceGrabber : MonoBehaviour
    {
        [SerializeField] private bool forNPC = true;
        [SerializeField, ShowIf("forNPC")] private NPC npc;

        [SerializeField] private bool forPlayer;
        [SerializeField, ShowIf("forPlayer")] private PlayerInventory playerInv;

        //private void Awake()
        //{
        //    layerMask = LayerMask.GetMask("ResourcePack");
        //}

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Collider entered" + other.name);
            ResourcePack resource = other.GetComponent<ResourcePack>();
            if (resource != null)
            {
                int amountAdded = 0;
                if (forNPC) npc.Inventory.Add(resource.Resource, (int)resource.Amount, out amountAdded);
                else if(forPlayer) playerInv.Inventory.Add(resource.Resource, (int)resource.Amount, out amountAdded);

                resource.Amount -= amountAdded;

                if(amountAdded > 0)
                    Debug.LogFormat("picked up {0}", resource.name);

                if (resource.Amount <= 0 && amountAdded != 0)
                    PoolManager.Instance.ReturnToPool(other.gameObject);
            }
        }
    }
}

