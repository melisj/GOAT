using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI;
using Goat.Storage;
using Goat.Grid.Interactions;
using System.Linq;

namespace Goat.AI.States
{
    public class PlaceItem : IState
    {
        private NPC npc;
        private Animator animator;
        public bool filledShelve;

        // Get this from npc
        private float placingSpeed = 0.5f, nextItemTime = 0;

        string storage = "Storage", container = "Container";

        public PlaceItem(NPC npc, Animator animator)
        {
            this.npc = npc;
            this.animator = animator;
        }

        private void PlaceItemInStorage()
        {  
            Resource resourceToPlace = null;

            if (npc.targetStorage.tag == storage)
                resourceToPlace = npc.targetStorage.MainResource;
            else if(npc.targetStorage.tag == container && npc.Inventory.ItemsInInventory > 0)
                resourceToPlace = npc.Inventory.Items.Keys.First();

            if(resourceToPlace != null && npc.targetStorage.Inventory.SpaceLeft > 0)
            {
                npc.targetStorage.Add(resourceToPlace, 1, out int amountPlaced);
                if (amountPlaced > 0)
                {
                    npc.Inventory.Remove(resourceToPlace, 1, out int amountRemoved);
                    animator.SetTrigger("Interact");
                }
                else
                    filledShelve = true;
            }
            else
            {
                filledShelve = true;
            }
        }

        public void Tick()
        {
            if(!filledShelve && nextItemTime <= Time.time)
            {
                PlaceItemInStorage();
                nextItemTime = Time.time + (1 / placingSpeed);
            }
        }

        public void OnEnter()
        {
            filledShelve = false;
        }

        public void OnExit()
        {

        }
    }
}

