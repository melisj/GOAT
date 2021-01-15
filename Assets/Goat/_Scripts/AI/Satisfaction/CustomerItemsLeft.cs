using UnityEngine;

namespace Goat.AI.Satisfaction
{
    public class CustomerItemsLeft : ReviewFactor
    {
        [SerializeField] private Customer customer;

        public override float GetReviewPoints()
        {
            float itemsFound = customer.AmountGroceries - customer.ItemsToGet.ItemsInInventory;

            //Found nothing
            if (itemsFound <= 0)
                return -customer.AmountGroceries * revData.Weight;
            else
                return itemsFound * revData.Weight;
            //Found some
        }
    }
}