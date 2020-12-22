using System.Collections;
using UnityEngine;

namespace Goat.AI.Parking
{
    public class WorkerSpawner : NPCSpawner
    {
        [SerializeField] private HiredEmployees hiredEmployees;

        /// <summary>
        /// Spawns all employees
        /// First looping through the amount hired
        /// Then moving on to the next category
        /// </summary>
        /// <param name="amountPassengers"></param>
        /// <returns></returns>
        protected override IEnumerator SpawnMultipleNPC(int amountPassengers = 2)
        {
            var looper = hiredEmployees.EmployeeList.GetEnumerator();
            int amountLeft = hiredEmployees.EmployeeList.Count;
            looper.MoveNext();
            int employeesLeft = looper.Current.Amount;

            while (amountLeft > 0)
            {
                yield return spawnDelaySeconds;
                if (!looper.Current) continue;

                if (employeesLeft <= 0)
                {
                    if (!looper.MoveNext()) break;
                    employeesLeft = looper.Current.Amount;
                }

                SpawnOneNPC(looper.Current.Prefab);
                employeesLeft--;
            }
        }
    }
}