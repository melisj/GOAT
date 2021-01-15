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
        [SerializeField] private AudioCue pickupItemAudio;
        //private void Awake()
        //{
        //    layerMask = LayerMask.GetMask("ResourcePack");
        //}

        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log("Collider entered" + other.name);
            ResourcePack resource = other.GetComponent<ResourcePack>();
            if (resource != null)
            {
                int amountAdded = 0;
                if (forNPC) npc.Inventory.Add(resource.Resource, 1, out amountAdded);
                else if (forPlayer) playerInv.Inventory.Add(resource.Resource, 1, out amountAdded);

                if (amountAdded == 1)
                {
                    pickupItemAudio.PlayAudioCue();
                    Debug.LogFormat("picked up {0}", resource.name);
                    PoolManager.Instance.ReturnToPool(other.gameObject);
                }
            }
        }
    }
}