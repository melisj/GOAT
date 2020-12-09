using System;
using UnityEngine;

namespace Goat.Storage
{
    [CreateAssetMenu(fileName = "Electricity", menuName = "ScriptableObjects/Electricity")]
    public class Electricity : ScriptableObject {
        public delegate void ElectricityChanged(int newUsed, int newCapacity);
        public event ElectricityChanged ElectricityChangedEvent;

        public EventHandler<int> ElecricityOverloaded;

        [SerializeField] private int capacity;
        [SerializeField] private int usedElectricity;

        public int UsedElectricity { get { return usedElectricity; } }
        public int Capacity { 
            get => capacity; 
            set {
                capacity = value;
                ElectricityChangedEvent?.Invoke(usedElectricity, capacity);

                if (usedElectricity > capacity) {
                    ElecricityOverloaded?.Invoke(this, usedElectricity - capacity);
                }
            } 
        }

        public bool AddElectricityConsumption(int consumption) {
            usedElectricity += consumption;

            // Check if electricity is overloaded
            if (usedElectricity > capacity) {
                ElecricityOverloaded?.Invoke(this, usedElectricity - capacity);
                usedElectricity -= consumption;
                return false;
            }

            ElectricityChangedEvent?.Invoke(usedElectricity, capacity);
            return true;
        }

        public void RemoveElectricityConsumption(int consumption) {
            usedElectricity -= consumption;
        }
    }
}