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
        private StockClerk stockClerk;
        private Animator animator;
        public bool filledShelve;

        public PlaceItem(StockClerk stockClerk, Animator animator)
        {
            this.stockClerk = stockClerk;
            this.animator = animator;
        }

        private void PlaceItemInStorageContainer()
        {
            Resource resourceToBePlaced = stockClerk.inventory.Keys.First();
            int amountToBePlaced = stockClerk.inventory[resourceToBePlaced];
            stockClerk.targetStorage.AddResource(resourceToBePlaced, amountToBePlaced, out int amountLeft);
            int amountPlaced = amountToBePlaced - amountLeft;
            stockClerk.RemoveResourceFromInventory(resourceToBePlaced, amountPlaced);
        }

        public void Tick()
        {
            //&& !(stockClerk.targetStorage.GetItemCount == stockClerk.targetStorage.GetMaxSpace)
            if (timeToFill <= Time.time )
            {
                //animated
                timeToFill = Time.time + (1 / fillingSpeed);
                PlaceItemInStorageContainer();
            }
            //if(stockClerk.targetStorage.GetItemCount == stockClerk.targetStorage.GetMaxSpace)
            //filledShelve = true;
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

