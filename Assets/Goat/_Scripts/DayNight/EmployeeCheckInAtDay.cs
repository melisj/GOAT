using Goat.AI.Parking;
using Goat.Events;
using UnityEngine;

namespace Goat
{
    public class EmployeeCheckInAtDay : EventListenerBool
    {
        [SerializeField] private HiredEmployees hiredEmployees;
        [SerializeField] private ShipSpawner shipSpawner;

        public override void OnEventRaised(bool value)
        {
            if (value && hiredEmployees.EmployeeList != null)
                shipSpawner.SpawnShip(hiredEmployees.EmployeeList.Count);
        }
    }
}