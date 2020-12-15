using Goat.Storage;
using UnityEngine;

namespace Goat.Events
{
    [System.Serializable]
    public class DeliveryResource
    {
        [SerializeField] private Buyable buyable;
        [SerializeField] private int amount;

        public DeliveryResource(Buyable resource, int amount)
        {
            this.buyable = resource;
            this.amount = amount;
        }

        public Buyable Buyable => buyable;
        public int Amount => amount;
    }
}