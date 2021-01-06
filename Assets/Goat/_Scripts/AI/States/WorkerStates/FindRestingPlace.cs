using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.AI.States
{
    public class FindRestingPlace : IState
    {
        public bool spotToChillFound;
        private Worker worker;
        public FindRestingPlace(Worker worker)
        {
            this.worker = worker;
        }

        public void Tick()
        {
        }

        public void OnEnter()
        {
            GameObject chillSpot = null;
            chillSpot = GameObject.Find("ChillTile");
            if (chillSpot != null)
            {
                worker.targetDestination = chillSpot.transform.position;
                spotToChillFound = true;
            }
        }

        public void OnExit()
        {
            spotToChillFound = false;
        }
    }
}

