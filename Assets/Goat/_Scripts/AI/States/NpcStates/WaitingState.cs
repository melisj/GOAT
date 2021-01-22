using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Goat.AI.States
{
    /// <summary>
    /// Do nothing in this state for a given time
    /// </summary>
    [System.Serializable]
    public class WaitingState : IState
    {
        private NPC npc;
        [SerializeField, ReadOnly] private float waitingTime, timeWaiting;
        [SerializeField, ReadOnly] private bool waiting;
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