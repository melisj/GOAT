using UnityEngine;

namespace Goat.AI.Satisfaction
{
    public class CustomerItemsLeft : ReviewFactor
    {
        [SerializeField] private NPC npc;

        public override float GetReviewPoints()
        {
            return -(npc.ItemsToGet.ItemsInInventory * reviewWeight.Weight);
        }
    }
}