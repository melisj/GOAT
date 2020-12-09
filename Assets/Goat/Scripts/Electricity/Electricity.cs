using Boo.Lang;
using Goat.Grid.Interactions;
using System;
using UnityEngine;

namespace Goat.Storage
{
    [CreateAssetMenu(fileName = "Electricity", menuName = "ScriptableObjects/Electricity")]
    public class Electricity : ScriptableObject {
        public delegate void ElectricityChanged(int newUsed, int newCapacity);
        public event ElectricityChanged ElectricityChangedEvent;
        public EventHandler<int> ElectricityOverloaded;

        [SerializeField] private List<BaseInteractable> poweredInteractables = new List<BaseInteractable>();

        [SerializeField] private int capacity;
        [SerializeField] private int usedElectricity;

        public int UsedElectricity { get { return usedElectricity; } }
        public int Capacity { 
            get => capacity; 
            set {
                capacity = value;
                ElectricityChangedEvent?.Invoke(usedElectricity, capacity);

                if (usedElectricity > capacity) {
                    DisableOverloadingInteractables();
                    ElectricityOverloaded?.Invoke(this, usedElectricity - capacity);
                }
            } 
        }

        public bool AddElectricityConsumption(BaseInteractable interactable) {
            usedElectricity += interactable.PowerCost;

            // Check if electricity is overloaded
            if (usedElectricity > capacity) {
                ElectricityOverloaded?.Invoke(this, usedElectricity - capacity);
                usedElectricity -= interactable.PowerCost;
                return false;
            }

            poweredInteractables.Add(interactable);
            ElectricityChangedEvent?.Invoke(usedElectricity, capacity);
            return true;
        }

        public void RemoveElectricityConsumption(BaseInteractable interactable) {
            poweredInteractables.Remove(interactable);
        }

        private void DisableOverloadingInteractables()
        {
            for(int i = poweredInteractables.Count - 1; i >= 0; i--)
            {
                if (usedElectricity > capacity)
                {
                    poweredInteractables[i].PowerOverloaded();
                    poweredInteractables.RemoveAt(i);
                }
                else
                    return;
            } 
        }

        public void ClearAll()
        {
            poweredInteractables.Clear();
            usedElectricity = 0;
            capacity = 0;
        }
    }
}