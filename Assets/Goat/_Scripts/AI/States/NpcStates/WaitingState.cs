using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.AI.States
{
    /// <summary>
    /// Do nothing in this state for a given time
    /// </summary>
    public class WaitingState : IState
    {
        private NPC npc;
        private float waitingTime, timeWaiting;
        private bool waiting;
        public bool Waiting => waiting;

        public WaitingState(NPC npc, float waitingTime)
        {
            this.npc = npc;
            this.waitingTime = waitingTime;
            waiting = true;
        }

        public void Tick()
        {
            if (timeWaiting <= Time.time)
                waiting = false;
        }

        public void OnEnter()
        {
            waiting = true;
            timeWaiting = Time.time + waitingTime;
        }

        public void OnExit()
        {
        }


    }
}

