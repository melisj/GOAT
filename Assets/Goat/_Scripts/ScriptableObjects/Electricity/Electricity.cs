using Boo.Lang;
using Goat.Grid.Interactions;
using System;
using UnityEngine;

namespace Goat.Storage
{
    [CreateAssetMenu(fileName = "Electricity", menuName = "ScriptableObjects/GlobalVariables/Electricity")]
    public class Electricity : ScriptableObject
    {
        public delegate void ElectricityChanged(int newUsed, int newCapacity);

        public event ElectricityChanged ElectricityChangedEvent;

        public EventHandler<int> ElectricityOverloaded;

        [SerializeField] private List<BaseInteractable> poweredInteractables = new List<BaseInteractable>();
        [SerializeField] private List<BaseInteractable> generatorInteractables = new List<BaseInteractable>();

        [SerializeField] private int capacity;
        [SerializeField] private int usedElectricity;
        [SerializeField] private int needElectricity;

        public bool IsOverCapacity => NeedElectricity > Capacity;
        public int UsedElectricity => usedElectricity;

        public int NeedElectricity
        {
            get => needElectricity;
            set
            {
                int tempNeed = needElectricity;
                needElectricity = value;
                if (tempNeed != value) ElectricityChangedEvent?.Invoke(value, capacity);
            }
        }

        public int Capacity
        {
            get => capacity;
            set
            {
                capacity = value;

                PowerInteractablesToCapacity();

                ElectricityChangedEvent?.Invoke(needElectricity, capacity);
            }
        }

        public List<BaseInteractable> GeneratorInteractables => generatorInteractables;

        public void AddDevice(BaseInteractable interactable)
        {
            if (!poweredInteractables.Contains(interactable))
            {
                poweredInteractables.Add(interactable);
                interactable.IsPowered = AddElectricityConsumption(interactable);
                PowerInteractablesToCapacity();
                NeedElectricity += interactable.PowerCost;
            }
        }

        public void RemoveDevice(BaseInteractable interactable)
        {
            if (poweredInteractables.Contains(interactable))
            {
                poweredInteractables.Remove(interactable);
                RemoveElectricityConsumption(interactable);
                PowerInteractablesToCapacity();
                NeedElectricity -= interactable.PowerCost;
            }
        }

        public void AddGenerator(BaseInteractable interactable)
        {
            generatorInteractables.Add(interactable);
            EnableGenerator(interactable);
        }

        public void RemoveGenerator(BaseInteractable interactable)
        {
            generatorInteractables.Remove(interactable);
            DisableGenerator(interactable);
        }

        public void EnableGenerator(BaseInteractable interactable)
        {
            Capacity += interactable.PowerProduction;
        }

        public void DisableGenerator(BaseInteractable interactable)
        {
            Capacity -= interactable.PowerProduction;
        }

        private bool AddElectricityConsumption(BaseInteractable interactable)
        {
            if (interactable.IsPowered) return true;

            int newUsage = usedElectricity + interactable.PowerCost;

            // Check if electricity is overloaded
            if (newUsage > capacity)
            {
                ElectricityOverloaded?.Invoke(this, newUsage - capacity);
                return false;
            }

            usedElectricity += interactable.PowerCost;
            return true;
        }

        private void RemoveElectricityConsumption(BaseInteractable interactable)
        {
            if (interactable.IsPowered)
                usedElectricity -= interactable.PowerCost;
            interactable.IsPowered = false;
        }

        private void PowerInteractablesToCapacity()
        {
            for (int i = 0; i < poweredInteractables.Count; i++)
            {
                if (usedElectricity <= capacity)
                    poweredInteractables[i].IsPowered = AddElectricityConsumption(poweredInteractables[i]);
                else
                    RemoveElectricityConsumption(poweredInteractables[i]);
            }
        }

        public void ClearAll()
        {
            poweredInteractables.Clear();
            usedElectricity = 0;
            needElectricity = 0;
            capacity = 0;
        }
    }
}