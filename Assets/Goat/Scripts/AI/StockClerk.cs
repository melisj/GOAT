using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI.States;

namespace Goat.AI
{
    public class StockClerk : NPC
    {

        protected override void Awake()
        {
            base.Awake();

            MoveToTarget moveToTarget = new MoveToTarget(this, targetDestination, navMeshAgent, animator);
            TakeItem takeItem = new TakeItem(this, animator, false);


        }
    }
}

