using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Events;
using Goat.Grid;
using UnityAtoms.BaseAtoms;

namespace Goat.AI.Satisfaction
{
    public class BeautyChecker : ReviewFactorWithEventListener<WithOwner<int>, IntEventWithOwner>
    {
        [SerializeField] private HashSet<Vector3> positions = new HashSet<Vector3>();
        [SerializeField] private List<Tile> tiles = new List<Tile>();
        [SerializeField] private Vector3HashSetEvent tileGiveEvent;
        private int totalBP;
        public HashSet<Vector3> Positions => positions;

        public void Setup()
        {
        }

        public override void OnEventRaised(WithOwner<int> value)
        {
            if (value.Owner == gameObject)
            {
                totalBP = value.Gtype;
            }
        }

        private void GetTiles()
        {
            tileGiveEvent.Raise(new WithOwner<HashSet<Vector3>>(positions, gameObject));
        }

        public override float GetReviewPoints()
        {
            GetTiles();

            return (totalBP * revData.Weight);
        }
    }
}