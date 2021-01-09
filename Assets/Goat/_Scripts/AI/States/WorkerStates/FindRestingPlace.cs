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

        private Vector3 ChillLocation()
        {
            Vector3 location = Vector3.zero;

            GameObject chillSpot = null;
            chillSpot = GameObject.Find("ChillTile");
            if (chillSpot != null)
            {
                location = chillSpot.transform.position;
                spotToChillFound = true;
            }

            return location;
        }

        public void OnEnter()
        {
            worker.targetDestination = ChillLocation();
        }

        public void OnExit()
        {
            spotToChillFound = false;
        }
    }
}

