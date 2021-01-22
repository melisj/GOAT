using UnityEngine;

namespace Goat.AI.Satisfaction
{
    public class CustomerSearchTime : ReviewFactor
    {
        [SerializeField] private NPC npc;

        public override float GetReviewPoints()
        {
            return -(npc.SearchingTime * revData.Weight);
        }
    }
}