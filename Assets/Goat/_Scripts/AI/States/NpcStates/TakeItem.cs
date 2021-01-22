using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Storage;
using Goat.Grid.Interactions;
using System;
using System.Linq;
using Sirenix.OdinInspector;

namespace Goat.AI.States
{
    /// <summary>
    /// Take item from a storage target
    /// </summary>
    [System.Serializable]
    public class TakeItem : IState
    {
        private NPC npc;
        private Animator animator;
        [SerializeField, ReadOnly] private bool returnToStock;
        [SerializeField, ReadOnly] private bool depleted;
        [SerializeField, ReadOnly] private float takingSpeed = 0.5f, nextItemTime = 0;
        // Get this from npc

        public bool Depleted => depleted;

        public event EventHandler eventHandler;

        public TakeItem(NPC npc, Animator animator, bool returnToStock)
        {
            this.npc = npc;
            this.animator = animator;
            this.returnToStock = returnToStock;
        }

        /// <summary>
        /// Take item from storage target if item is available
        /// </summary>
        private void TakeItemFromStorage()
        {
            // If target still has item grab item.
            bool nothingFound = true;
            for (int i = 0; i < npc.TargetStorage.Inventory.Items.Count; i++)
            {
                Resource tempResource = npc.TargetStorage.Inventory.Items.ElementAt(i).Key;
                if (npc.ItemsToGet.Contains(tempResource))
                {
                    if (npc is Customer customer)
                        customer.TotalPriceProducts += tempResource.Price(true);

                    npc.Inventory.Add(tempResource, 1, out int amountStored);
                    npc.ItemsToGet.Remove(tempResource, amountStored, out int itemsRemoved);
                    npc.TargetStorage.Inventory.Remove(tempResource, amountStored, out int storageRemoved);

                    animator.SetTrigger("Interact");
                    eventHandler?.Invoke(this, null);

                    nothingFound = false;

                    break;
                }
            }
            depleted = nothingFound;
        }

        public void Tick()
        {
            npc.SearchingTime += Time.deltaTime;

            // If time to grab next item
            if (!Depleted && nextItemTime <= Time.time)
            {
                nextItemTime = Time.time + (1 / takingSpeed);
                TakeItemFromStorage();
            }
        }

        public void OnEnter()
        {
            depleted = false;
        }

        public void OnExit()
        {
            npc.TargetStorage = null;
            npc.TargetDestination = npc.transform.position;
        }
    }
}