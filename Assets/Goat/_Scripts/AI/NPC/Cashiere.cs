using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.AI.States;

namespace Goat.AI
{
    public class Cashiere : NPC
    {

        protected override void Setup()
        {
            base.Setup();

            MoveToDestination moveToDestination = new MoveToDestination(this, navMeshAgent);

            Func<bool> ReachedDestination() => () => navMeshAgent.remainingDistance < npcSize / 2;

            void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);

        }
    }
}

