using Goat.ScriptableObjects;
using Goat.Storage;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Player
{
    [CreateAssetMenu(fileName = "PlayerInventory", menuName = "ScriptableObjects/RuntimeVariables/PlayerInventory")]
    public class PlayerInventory : ScriptableObject
    {
        private Inventory inventory;
        public Inventory Inventory
        {
            get => inventory;
            set => inventory = value;
        }

        [SerializeField, ReadOnly] private int currentCapacity;
        [SerializeField] private int defaultCapacity;

        [SerializeField] private bool gamemodeCreative;
        [SerializeField, ShowIf("gamemodeCreative")] private ResourceArray resources;

        public void InitInventory()
        {
            if(gamemodeCreative)
                InitCreativeInventory();
            else
            {
                currentCapacity = defaultCapacity;
                if (currentCapacity == 0) Debug.LogError("Player inventory has zero capacity!!! Please set the default capacity.");
                inventory = new Inventory(currentCapacity);
            }
        }

        private void InitCreativeInventory()
        {
            currentCapacity = int.MaxValue;
            inventory = new Inventory(currentCapacity);

            for (int i = 0; i < resources.Resources.Length; i++)
            {
                Inventory.Add(resources.Resources[i], 666, out int storedAmount);
            }

            inventory.InfiniCapacity = true;
        }

        [Button("Clear Inventory")]
        public void ClearInventory()
        {
            Inventory.Clear();
        }

        public void TryGetValue(Resource resource, out int amount)
        {
            Inventory.Items.TryGetValue(resource, out amount);
        }
    }
}