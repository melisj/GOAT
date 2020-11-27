using Goat.Storage;
using GOAT.Grid.UI;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GOAT.Grid
{
    /// <summary>
    /// Storage object placed on the grid
    /// Contains info about the resources it holds
    /// Contains info about its enviroment conditions
    /// </summary>
    public class StorageInteractable : BaseInteractable
    {
        [SerializeField] private List<ItemInstance> resourceList = new List<ItemInstance>();
        private readonly int maxResource = 8;

        [SerializeField] private StorageEnviroment enviroment;

        // Properties
        private int GetResourceCount { get { return resourceList.Count; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"> List of items being stored</param>
        /// <returns> Returns true if an item has been stored, false if none are stored </returns>
        public bool AddResource(ref List<ItemInstance> items) {
            int amountWantingToBeStored = items.Count();
            int amountBeingStored = Mathf.Min(Mathf.Abs(GetResourceCount - maxResource), amountWantingToBeStored);

            // Check if there is space left
            if (amountBeingStored == 0)
                return false;

            // Store items in the list
            for (int i = amountBeingStored - 1; i >= 0; i--) {
                resourceList.Add(items[i]);
                items.RemoveAt(i);
            }

            InvokeChange();

            return true;
        } 

        /// <summary>
        /// Get an item with given index
        /// </summary>
        /// <param name="index"> Index of the item you want </param>
        /// <returns> Returns the selected item </returns>
        public ItemInstance GetResource(int index) {
            ItemInstance items = resourceList[index];
            resourceList.RemoveAt(index);

            InvokeChange();
            return items;
        }

        // Show UI
        public override void OpenUI() {
            base.OpenUI();
        }

        // Update the text of the UI
        protected override void UpdateUI() {
            base.UpdateUI();

            InteractableManager.instance.interactableUI.LoadElement(InteractableUIElement.Storage);
            object[] obj = { string.Format("Storage -=- {0}/{1}", GetResourceCount, maxResource), resourceList.ToList(), this };
            InteractableManager.instance.interactableUI.SetElementValue(obj);
        }


        // Test
        public ItemInstance[] instances;

        [Button("test add")]
        public void TestAdd() {
            List<ItemInstance> insc = instances.ToList();
            AddResource(ref insc);
        }
    }
}