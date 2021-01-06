using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Events;
using UnityAtoms.BaseAtoms;

namespace Goat.Grid
{
    public class TileGiver : EventListenerVector3HashSetEvent
    {
        [SerializeField] private IntEventWithOwner beautyPointEvent;
        [SerializeField] private Grid grid;

        public override void OnEventRaised(WithOwner<HashSet<Vector3>> value)
        {
            int totalBP = 0;
            var looper = value.Gtype.GetEnumerator();
            while (looper.MoveNext())
            {
                Tile tile = grid.ReturnTile(grid.CalculateTilePositionInArray(looper.Current));
                if (tile != null)
                    totalBP += tile.TotalBeautyPoints;
            }
            beautyPointEvent.Raise(new WithOwner<int>(totalBP, value.Owner));
        }
    }
}