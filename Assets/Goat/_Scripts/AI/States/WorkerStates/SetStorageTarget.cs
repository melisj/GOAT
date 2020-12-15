using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Grid.Interactions;

namespace Goat.AI.States
{
    public class SetStorageTarget : IState
    {
        private StockClerk stockClerk;
        public SetStorageTarget(StockClerk stockClerk)
        {
            this.stockClerk = stockClerk;
        }

        public void Tick()
        {

        }

        private StorageInteractable SetNewTarget()
        {
            StorageInteractable tempStorage;
            if(stockClerk.targetStorages != null && stockClerk.targetStorages.Count > 0)
            {
                tempStorage = stockClerk.targetStorages[0];
                stockClerk.targetStorages.RemoveAt(0);
                return tempStorage;
            }
            else
            {
                Debug.LogError("Why are you setting a target storage when there are no targets?");
                return null;
            }
        }

        public void OnEnter()
        {
            stockClerk.targetStorage = SetNewTarget();
        }

        public void OnExit()
        {

        }
    }
}

