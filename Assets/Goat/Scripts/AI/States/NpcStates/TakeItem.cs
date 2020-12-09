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
        Animator animator;
        bool returnToStock;
        public bool storageDepleted;
        // Get this from npc
        private float takingSpeed = 0.5f, nextItemTime = 0;

        public TakeItem(NPC npc, Animator animator, bool returnToStock)
        {
            this.npc = npc;
            this.animator = animator;
            this.returnToStock = returnToStock;
        }

        private void TakeItemFromStorage()
        {
            // If target still has item grab item.
            bool nothingFound = true;
            for (int i = 0; i < npc.targetStorage.GetItemCount; i++)
            {
                if (npc.itemsToGet.ContainsKey(npc.targetStorage.GetItems[i].Resource))
                {
                    Debug.LogFormat("Took {0} from storage container", npc.targetStorage.GetItems[i].Resource);
                    npc.AddResourceToInventory(npc.targetStorage.GetItems[i].Resource, 1);
                    npc.RemoveItemToGet(npc.targetStorage.GetItems[i].Resource, 1);
                    npc.targetStorage.GetResource(i, returnToStock);
                    nothingFound = false;

                    if (npc is Customer)
                        ((Customer)npc).totalPriceProducts += npc.targetStorage.GetItems[i].Resource.Price;

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
            Debug.Log("Started taking items");
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

