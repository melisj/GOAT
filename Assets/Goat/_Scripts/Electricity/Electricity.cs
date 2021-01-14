using Boo.Lang;
using Goat.Grid.Interactions;
using System;
using UnityEngine;

namespace Goat.Farming.Electricity
{
    [CreateAssetMenu(fileName = "Electricity", menuName = "ScriptableObjects/GlobalVariables/Electricity")]
    public class Electricity : ScriptableObject
    {
        public delegate void ElectricityChanged(int newUsed, int newCapacity);

        public event ElectricityChanged ElectricityChangedEvent;

        public EventHandler<int> ElectricityOverloaded;

        [SerializeField] private List<ElectricityComponent> poweredInteractables = new List<ElectricityComponent>();
        [SerializeField] private List<ElectricityComponent> generatorInteractables = new List<ElectricityComponent>();

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

        public List<ElectricityComponent> GeneratorInteractables => generatorInteractables;

        public void AddDevice(ElectricityComponent interactable)
        {
            if (!poweredInteractables.Contains(interactable))
            {
                poweredInteractables.Add(interactable);
                interactable.IsPowered = AddElectricityConsumption(interactable);
                PowerInteractablesToCapacity();
                NeedElectricity += interactable.PowerCost;
            }
        }

        public void RemoveDevice(ElectricityComponent interactable)
        {
            if (poweredInteractables.Contains(interactable))
            {
                poweredInteractables.Remove(interactable);
                RemoveElectricityConsumption(interactable);
                PowerInteractablesToCapacity();
                NeedElectricity -= interactable.PowerCost;
            }
        }

        public void AddGenerator(ElectricityComponent interactable)
        {
            generatorInteractables.Add(interactable);
            EnableGenerator(interactable);
        }

        public void RemoveGenerator(ElectricityComponent interactable)
        {
            generatorInteractables.Remove(interactable);
            DisableGenerator(interactable);
        }

        public void EnableGenerator(ElectricityComponent interactable)
        {
            Capacity += interactable.PowerProduction;
        }

        public void DisableGenerator(ElectricityComponent interactable)
        {
            Capacity -= interactable.PowerProduction;
        }

        private bool AddElectricityConsumption(ElectricityComponent interactable)
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

        private void RemoveElectricityConsumption(ElectricityComponent interactable)
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