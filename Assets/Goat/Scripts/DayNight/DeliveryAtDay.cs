using Goat.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat
{
    public class DeliveryAtDay : EventListenerBool
    {
        [SerializeField] private ShipCreator shipCreator;

        public override void OnEventRaised(bool value)
        {
            if (value)
                shipCreator.CreateCargoShip();
        }
    }
}