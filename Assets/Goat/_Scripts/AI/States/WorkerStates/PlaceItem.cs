using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI;
using Goat.Storage;
using Goat.Grid.Interactions;
using System.Linq;
using System;

namespace Goat.AI.States
{
    public class PlaceItem : IState
    {
        private Worker worker;
        private Animator animator;
        public bool filled = false;

        // Get this from npc
        private float placingSpeed = 0.5f, nextItemTime = 0;

        private string storage = "Storage", container = "Container";

        public EventHandler eventHandler;

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
            else if (worker.targetStorage.tag == container && worker.Inventory.ItemsInInventory > 0)
                resourceToPlace = worker.Inventory.Items.First().Key;

            if (resourceToPlace == null)
            {
                filled = true;
                return;
            }

            if (worker.targetStorage.Inventory.SpaceLeft > 0 && worker.Inventory.Contains(resourceToPlace))
            {
                animator.SetTrigger("Interact");
                //Delay
                eventHandler.Invoke(this, null);
                    
                worker.targetStorage.Inventory.Add(resourceToPlace, 1, out int amountPlaced);
                worker.Inventory.Remove(resourceToPlace, amountPlaced, out int amountRemoved);
                Debug.LogFormat("Placed {0} in storage", resourceToPlace.name);
            }
            else
                filled = true;
        }

        public void Tick()
        {
            if (!filled && nextItemTime <= Time.time)
            {
                PlaceItemInStorage();
                nextItemTime = Time.time + (1 / placingSpeed);
            }
        }

        public void OnEnter()
        {
            filled = false;
            animator.speed = 2 * placingSpeed;
        }

        public void OnExit()
        {
            worker.targetStorage.selected = false;
            worker.targetStorage = null;
            animator.speed = 1;
        }
    }
}