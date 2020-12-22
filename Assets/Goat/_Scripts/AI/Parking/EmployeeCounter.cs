using Goat.Events;
using UnityEngine;
using System.Collections.Generic;

namespace Goat.AI.Parking
{
    public class EmployeeCounter : EventListenerHiredEmployee
    {
        [SerializeField] private HiredEmployees hiredEmployees;

        public override void OnEventRaised(HiredEmployee value)
        {
            if (hiredEmployees.EmployeeList == null)
            {
                hiredEmployees.EmployeeList = new HashSet<HiredEmployee>();
            }
            hiredEmployees.EmployeeList.Add(value);
        }
    }
}