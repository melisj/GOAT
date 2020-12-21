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
        private Worker worker;
        private Animator animator;
        public bool filledShelve = false;

        // Get this from npc
        private float placingSpeed = 0.5f, nextItemTime = 0;

        string storage = "Storage", container = "Container";

        public PlaceItem(Worker worker, Animator animator)
        {
            this.worker = worker;
            this.animator = animator;
        }

        private void PlaceItemInStorage()
        {  
            Resource resourceToPlace = null;

            if (worker.targetStorage.tag == storage)
                resourceToPlace = worker.targetStorage.MainResource;
            else if(worker.targetStorage.tag == container && worker.Inventory.ItemsInInventory > 0)
                resourceToPlace = worker.Inventory.Items.First().Key;

            if(resourceToPlace == null)
            {
                filledShelve = true;
                return;
            }

            if(worker.targetStorage.Inventory.SpaceLeft > 0 && worker.Inventory.Contains(resourceToPlace))
            {
                worker.targetStorage.Add(resourceToPlace, 1, out int amountPlaced);
                if (amountPlaced > 0)
                {
                    Debug.LogFormat("Placed {0} in storage", resourceToPlace.name);
                    worker.Inventory.Remove(resourceToPlace, 1, out int amountRemoved);
                    animator.SetTrigger("Interact");
                }
                else
                    filledShelve = true;
            }
            else
                filledShelve = true;

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
            worker.targetStorage = null;
        }
    }
}

