using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI;
using Goat.Storage;
using Goat.Grid.Interactions;
using System.Linq;
using System;
using Sirenix.OdinInspector;

namespace Goat.AI.States
{
    /// <summary>
    /// Place item from NPC inventory into a storage inventory
    /// </summary>
    [System.Serializable]
    public class PlaceItem : IState
    {
        private const string STORAGETAG = "Storage", CONTAINERTAG = "Container";

        private Worker worker;
        private Animator animator;
        private float placingSpeed;

        // Get this from npc
        [SerializeField, ReadOnly] private bool filled;
        [SerializeField, ReadOnly] private float nextItemTime = 0;

        public EventHandler eventHandler;
        public bool Filled => filled;

        public PlaceItem(Worker worker, Animator animator, float placingSpeed)
        {
            this.worker = worker;
            this.animator = animator;
            this.placingSpeed = placingSpeed;
        }

        /// <summary>
        /// Set item to place based on worker type and place that item in storage
        /// </summary>
        private void PlaceItemInStorage()
        {
            Resource resourceToPlace = null;

            if (worker.TargetStorage.CompareTag(STORAGETAG))
                resourceToPlace = worker.TargetStorage.MainResource;
            else if (worker.TargetStorage.CompareTag(CONTAINERTAG) && worker.Inventory.ItemsInInventory > 0)
                resourceToPlace = worker.Inventory.Items.First().Key;

            if (resourceToPlace == null)
            {
                filled = true;
                return;
            }

            if (worker.TargetStorage.Inventory.SpaceLeft > 0 && worker.Inventory.Contains(resourceToPlace))
            {
                animator.SetTrigger("Interact");
                //Delay
                eventHandler?.Invoke(this, null);

                worker.TargetStorage.Inventory.Add(resourceToPlace, 1, out int amountPlaced);
                worker.Inventory.Remove(resourceToPlace, amountPlaced, out int amountRemoved);
                Debug.LogFormat("Placed {0} in storage", resourceToPlace.name);
            }
            else
                filled = true;
        }

        public void Tick()
        {
            if (!Filled && nextItemTime <= Time.time)
            {
                nextItemTime = Time.time + (1 / placingSpeed);
                PlaceItemInStorage();
            }
        }

        public void OnEnter()
        {
            filled = false;
            animator.speed = 2 * placingSpeed;
        }

        public void OnExit()
        {
            worker.TargetStorage.selected = false;
            worker.TargetStorage = null;
            animator.speed = 1;
        }
    }
}