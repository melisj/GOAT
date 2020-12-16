using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Events;

namespace Goat.Grid
{
    public class TileGiver : EventListenerVector3
    {
        [SerializeField] private TileDataEvent tileDataEvent;
        [SerializeField] private Grid grid;

        public override void OnEventRaised(Vector3 value)
        {
            tileDataEvent.Raise(grid.ReturnTile(grid.CalculateTilePositionInArray(value)));
        }
    }
}