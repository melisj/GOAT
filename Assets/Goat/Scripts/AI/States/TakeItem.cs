using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Storage;
using Goat.Grid.Interactions;
using System;

namespace Goat.AI.States
{
    public class TakeItem : IState
    {
        NPC npc;
        StorageInteractable storageUnit;
        Animator animator;
        Dictionary<Resource, int> itemsToGet;
        bool returnToStock;
        public bool storageDepleted;
        // Get this from npc
        private float takingSpeed = 1, nextItemTime = 0;

        public TakeItem(NPC npc, StorageInteractable storageUnit, Animator animator, bool returnToStock)
        {
            this.npc = npc;
            this.storageUnit = storageUnit;
            this.animator = animator;
            this.itemsToGet = this.npc.itemsToGet;
            this.returnToStock = returnToStock;
        }

        private void TakeItemFromStorage()
        {
            // If target still has item grab item.
            bool nothingFound = true;
            for (int i = 0; i < storageUnit.GetItemCount; i++)
            {
                if (itemsToGet.ContainsKey(storageUnit.GetItems[i].Resource))
                {
                    npc.AddResourceToInventory(storageUnit.GetItems[i].Resource, 1);
                    npc.RemoveItemToGet(storageUnit.GetItems[i].Resource, 1);
                    storageUnit.GetResource(i, returnToStock);
                    nothingFound = false;
                    break;
                }
            }
            storageDepleted = nothingFound;
        }

        public void Tick()
        {
            // If time to grab next item
            if (!storageDepleted && nextItemTime <= Time.time)
            {
                nextItemTime = Time.time + (1 / takingSpeed);
                TakeItemFromStorage();
                // Animate
            }
        }

        public void OnEnter()
        {
            // Start animation?
            storageDepleted = false;
        }

        public void OnExit()
        {
            // End animation?
            npc.targetStorage = null;
            npc.targetDestination = npc.transform.position;
            npc.searchingTime = Time.time - npc.enterTime;

        }
    }
}

