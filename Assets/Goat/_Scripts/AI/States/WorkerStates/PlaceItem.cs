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

        private float fillingSpeed = 1, timeToFill = 0;
        private NPC npc;
        private Animator animator;
        public bool filledShelve;

        private Resource resourceToBePlaced;

        public PlaceItem(NPC npc, Animator animator)
        {
            this.npc = npc;
            this.animator = animator;
        }

        // WarehouseWorker
        private void PlaceItemInStorageContainer()
        {
            // Rewrite this shit
            //Resource resourceToBePlaced = npc.inventory.Keys.First();
            //int amountToBePlaced = npc.inventory[resourceToBePlaced];
            //npc.targetStorage.AddResource(resourceToBePlaced, amountToBePlaced, out int amountLeft);
            //int amountPlaced = amountToBePlaced - amountLeft;
            //npc.RemoveResourceFromInventory(resourceToBePlaced, amountPlaced);

            // Has to work for big storage containers which don't exist yet

        }
        // StockClerk
        private void PlaceItemOnShelves()
        {
            if(npc.targetStorage.SpaceLeft > 0 && npc.inventory.ContainsKey(resourceToBePlaced))
            {
                npc.targetStorage.AddResource(resourceToBePlaced, 1, out int amountPlaced);
                npc.RemoveResourceFromInventory(resourceToBePlaced, amountPlaced);
                animator.SetTrigger("Interact");
            }
            else
            {
                filledShelve = true;
            }
        }

        public void Tick()
        {
            //&& !(stockClerk.targetStorage.GetItemCount == stockClerk.targetStorage.GetMaxSpace)
            if (timeToFill <= Time.time && !filledShelve)
            {
                timeToFill = Time.time + (1 / fillingSpeed);

                if (npc is StockClerk)
                    PlaceItemOnShelves();
                else if (npc is WarehouseWorker)
                    PlaceItemInStorageContainer();
            }
            //if(stockClerk.targetStorage.GetItemCount == stockClerk.targetStorage.GetMaxSpace)
            //filledShelve = true;
        }

        public void OnEnter()
        {
            filledShelve = false;
            if(npc is StockClerk)
                resourceToBePlaced = npc.targetStorage.MainResource;
        }

        public void OnExit()
        {

        }
    }
}

