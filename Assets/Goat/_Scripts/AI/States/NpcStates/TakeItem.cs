using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Storage;
using Goat.Grid.Interactions;
using System;
using System.Linq;

namespace Goat.AI.States
{
    public class TakeItem : IState
    {
        NPC npc;
        Animator animator;
        bool returnToStock;
        public bool depleted = false;
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
            for (int i = 0; i < npc.targetStorage.Inventory.Items.Count; i++)
            {
                Resource tempResource = npc.targetStorage.Inventory.Items.ElementAt(i).Key;
                if (npc.ItemsToGet.Contains(tempResource))
                {
                    if (npc is Customer)
                        ((Customer)npc).totalPriceProducts += tempResource.Price;

                    Debug.LogFormat("Took {0} from storage container", tempResource.name);
                    npc.Inventory.Add(tempResource, 1, out int amountStored);
                    npc.ItemsToGet.Remove(tempResource, amountStored, out int itemsRemoved);
                    npc.targetStorage.Inventory.Remove(tempResource, amountStored, out int storageRemoved);

                    animator.SetTrigger("Interact");

                    nothingFound = false;

                    break;
                }
            }
            depleted = nothingFound;
        }

        public void Tick()
        {
            npc.searchingTime += Time.deltaTime;

            // If time to grab next item
            if (!depleted && nextItemTime <= Time.time)
            {
                nextItemTime = Time.time + (1 / takingSpeed);
                TakeItemFromStorage();
            }

        }

        public void OnEnter()
        {
            Debug.Log("Started taking items");
            // Start animation?
            depleted = false;
            storageDepleted = false;
        }

        public void OnExit()
        {
            // End animation?
            npc.targetStorage = null;
            npc.targetDestination = npc.transform.position;
        }
    }
}

