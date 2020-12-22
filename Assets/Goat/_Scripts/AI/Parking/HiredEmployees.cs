using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Goat.AI.Parking
{
    [CreateAssetMenu(fileName = "HiredEmployees", menuName = "ScriptableObjects/RuntimeVariables/HiredEmployees")]
    public class HiredEmployees : SerializedScriptableObject
    {
        [SerializeField, ReadOnly] private HashSet<HiredEmployee> employees = new HashSet<HiredEmployee>();

        public HashSet<HiredEmployee> EmployeeList { get => employees; set => employees = value; }

        public int GetEmployeeCount()
        {
            var looper = employees.GetEnumerator();
            int employeeCount = 0;
            while (looper.MoveNext())
            {
                employeeCount += looper.Current.Amount;
            }

            return employeeCount;
        }

        [Button]
        private void Clear()
        {
            employees.Clear();
        }
    }
}