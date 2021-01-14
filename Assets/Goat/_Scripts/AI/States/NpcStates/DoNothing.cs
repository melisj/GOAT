using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.AI.States
{
    /// <summary>
    /// Do nothing in this state
    /// </summary>
    public class DoNothing : IState
    {
        NPC npc;

        public DoNothing(NPC npc)
        {
            this.npc = npc;
        }

        public void Tick()
        {
            npc.searchingTime += Time.deltaTime;
        }

        public void OnEnter()
        {
            Debug.Log("Doing nothing");
            if (npc is WarehouseWorker)
                ((WarehouseWorker)npc).searching = true;
            npc.targetDestination = npc.transform.position;
        }

        public void OnExit()
        {
            if (npc is WarehouseWorker)
                ((WarehouseWorker)npc).searching = false;
        }
    }
}

