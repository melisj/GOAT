using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.AI.States
{
    public class DoNothing : IState
    {
        NPC npc;

        public DoNothing(NPC npc)
        {
            this.npc = npc;
        }

        public void Tick()
        {

        }

        public void OnEnter()
        {
            Debug.Log("Doing nothing");
        }

        public void OnExit()
        {

        }
    }
}

