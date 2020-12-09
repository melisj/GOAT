using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI;
using Goat.Grid.Interactions;

namespace Goat.AI.States
{
    public class SearchForEmptyShelves : IState
    {
        StockClerk stockClerk;

        public SearchForEmptyShelves(StockClerk stockClerk)
        {
            this.stockClerk = stockClerk;
        }

        public void Tick()
        {

        }

        public void OnEnter()
        {

        }

        public void OnExit()
        {

        }
    }
}

