using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Events;
using Goat.Grid;

namespace Goat.AI.Satisfaction
{
    public class BeautyChecker : ReviewFactorWithEventListener<Tile, TileDataEvent>
    {
        [SerializeField] private HashSet<Tile> tiles = new HashSet<Tile>();

        public void Setup()
        {
            tiles.Clear();
        }

        public override void OnEventRaised(Tile value)
        {
            tiles.Add(value);
        }

        public override float GetReviewPoints()
        {
            int totalBP = 0;

            var looper = tiles.GetEnumerator();

            while (looper.MoveNext())
            {
                if(looper.Current != null)
                    totalBP += looper.Current.TotalBeautyPoints;
            }

            return (totalBP * reviewWeight.Weight);
        }
    }
}