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
            get
            {
                if (inventory == null) inventory = new Inventory(currentCapacity);
                return inventory;
            }
            set => inventory = value;
        }

        [SerializeField, ReadOnly] private int currentCapacity;
        [SerializeField] private int defaultCapacity;

        [SerializeField] private bool gamemodeCreative;
        [SerializeField, ShowIf("gamemodeCreative")] private ResourceArray resources;

        public void InitInventory()
        {
            currentCapacity = defaultCapacity;

            if (gamemodeCreative) currentCapacity = int.MaxValue;
            if (currentCapacity == 0) Debug.LogError("Player inventory has zero capacity!!! Please set the default capacity.");

            inventory = new Inventory(currentCapacity);
            if (gamemodeCreative)
            {
                for (int i = 0; i < resources.Resources.Length; i++)
                {
                    inventory.Add(resources.Resources[i], 666, out int storedAmount);
                } 
            }
        }

        public void TryGetValue(Resource resource, out int amount)
        {
            Inventory.Items.TryGetValue(resource, out amount);
        }
    }
}