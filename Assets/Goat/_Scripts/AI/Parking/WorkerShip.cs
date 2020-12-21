using Goat.Pooling;
using UnityEngine;

namespace Goat.AI.Parking
{
    public class WorkerShip : NPCShip
    {
        [SerializeField] private HiredEmployees hiredEmployees;
        [SerializeField] private int amountOnShip;
        private int employeeCount;

        public override void ShipReadyToFly()
        {
            amountOnShip++;
            if (amountOnShip >= employeeCount)
            {
                base.ShipReadyToFly();
            }
        }

        protected override void ShipHasLanded()
        {
            AmountPassengers = employeeCount;
            base.ShipHasLanded();
        }

        public override void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            base.OnGetObject(objectInstance, poolKey);
            amountOnShip = 0;
            employeeCount = hiredEmployees.GetEmployeeCount();
        }
    }
}