using Goat.Storage;
using Goat.Grid.UI;
using Goat.Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Goat.Grid.Interactions
{
    /// <summary>
    /// Storage object placed on the grid
    /// Contains info about the resources it holds
    /// Contains info about its enviroment conditions
    /// </summary>
    public class StorageInteractable : BaseInteractable
    {
        [SerializeField] private List<ItemInstance> itemList = new List<ItemInstance>();
        [SerializeField] private int maxResources = 4;
        [SerializeField] private StorageEnviroment enviroment;

        private ItemInstance[] itemPhysicalHolderArray;
        private List<MeshFilter> itemHolderMeshList = new List<MeshFilter>();

        // Properties
        public int GetItemCount { get => itemList.Count; }

        public int SpaceLeft { get => Mathf.Abs(GetItemCount - maxResources); }

        public List<ItemInstance> GetItems { get => itemList; }

        public ItemInstance[] PhysicalItemList
        {
            get => itemPhysicalHolderArray;
            set
            {
                itemPhysicalHolderArray = value;
                itemList = itemPhysicalHolderArray.ToList();
                itemList.RemoveAll((item) => item == null);
                InformationChanged.Invoke();
            }
        }

        private object[] GetArgsForUI
        {
            get => new object[] {
            string.Format("Storage -=- {0}/{1}", GetItemCount, maxResources),
            itemList.ToList(),
            this };
        }

        // Get or create a item holder object
        private Transform ItemHolderParent;

        private Transform GetItemHolderParent
        {
            get
            {
                ItemHolderParent = transform.Find(InteractableManager.ItemHolderParentName) ??
                    new GameObject(InteractableManager.ItemHolderParentName).transform;

                if (ItemHolderParent.parent != transform)
                    ItemHolderParent.SetParent(transform, false);

                return ItemHolderParent;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            InformationChanged.AddListener(UpdateVisuals);
            InitStorage();
        }

        private void OnDestroy()
        {
            GetAllResources();
            NpcManager.Instance.RemoveStorageShelve(this);
        }

        private void Awake()
        {
            ResetStorage();
            NpcManager.Instance.AddStorageShelve(this);
        }

        #region Item Holders

        /// <summary>
        /// Initialize the holder items for the objects
        /// </summary>
        [Button("Generate/Get Item Holders")]
        private void InitStorage()
        {
            itemHolderMeshList.Clear();
            for (int i = 0; i < maxResources; i++)
            {
                GetItemHolder(i);
            }
        }

        // Try to get a item holder from the parent
        private void GetItemHolder(int index)
        {
            GameObject itemHolder = null;
            try
            {
                itemHolder = GetItemHolderParent.GetChild(index).gameObject;
            }
            catch
            {
                itemHolder = CreateItemHolder();
            }

            // Set material and save the meshfilter
            itemHolder.GetComponent<MeshRenderer>().material = InteractableManager.ItemMaterial;
            itemHolderMeshList.Add(itemHolder.GetComponent<MeshFilter>());
        }

        // Create a new item holder
        private GameObject CreateItemHolder()
        {
            GameObject itemHolder = new GameObject(InteractableManager.ItemHolderName, typeof(MeshFilter), typeof(MeshRenderer));
            itemHolder.transform.SetParent(GetItemHolderParent, false);
            return itemHolder;
        }

        // Render out locations where meshes will be placed from the inventory
        private void OnDrawGizmos()
        {
            if (itemHolderMeshList != null)
            {
                Gizmos.color = Color.green;
                foreach (MeshFilter mesh in itemHolderMeshList)
                {
                    Gizmos.DrawSphere(mesh.transform.position, 0.1f);
                }
            }
        }

        #endregion Item Holders

        #region Inventory Management

        /// <summary>
        /// Add already instatiated instances of items
        /// </summary>
        /// <param name="items"> List of items being stored (removes the ones it is able to store)</param>
        /// <returns> Returns true if an item has been stored, false if none are stored </returns>
        public bool AddResource(ref List<ItemInstance> items)
        {
            int amountWantingToBeStored = items.Count();
            int amountBeingStored = Mathf.Min(SpaceLeft, amountWantingToBeStored);

            // Check if there is space left
            if (amountBeingStored == 0)
                return false;

            // Store items in the list
            for (int i = amountBeingStored - 1; i >= 0; i--)
            {
                itemList.Add(items[i]);
                NpcManager.Instance.AddAvailableResource(items[i].Resource.ResourceType, 1);
                AddPhysicalMesh(items[i]);
                items.RemoveAt(i);
            }

            InvokeChange();

            return true;
        }

        /// <summary>
        /// Add resources based on amount and type of resource
        /// </summary>
        /// <param name="type"> Give type it should generate </param>
        /// <param name="amount"> Give amount it should generate </param>
        /// <param name="storedAmount"> Outs the amount that was actually stored </param>
        /// <returns> Return whether it stored at least one item </returns>
        public bool AddResource(Resource type, int amount, out int storedAmount)
        {
            int amountBeingStored = Mathf.Min(SpaceLeft, amount);
            List<ItemInstance> items = new List<ItemInstance>();

            if (amount != amountBeingStored)
                Debug.LogWarningFormat("Could not add {0} of type {1} to inventory!", amount - amountBeingStored, type.ResourceType);

            // Store items in the list
            for (int i = 0; i < amount; i++)
            {
                items.Add(new ItemInstance(type));
            }

            storedAmount = amountBeingStored;
            return AddResource(ref items);
        }

        /// <summary>
        /// Get an item with given index
        /// Removes item from the storage and returns it
        /// </summary>
        /// <param name="index"> Index of the item you want </param>
        /// <param name="returnToStock"> Return the item to the stock by adding to the resources </param>
        /// <returns> Returns the selected item </returns>
        public ItemInstance GetResource(int index, bool returnToStock = true)
        {
            ItemInstance item = itemList[index];
            NpcManager.Instance.RemoveAvailableResource(item.Resource.ResourceType, 1);
            itemList.RemoveAt(index);
            RemovePhysicalMesh(item);

            if (returnToStock)
                item.Resource.Amount++;

            InvokeChange();
            return item;
        }

        /// <summary>
        /// Returns all items from the storage
        /// Remove all items from the shelf
        /// </summary>
        /// <param name="index"> Index of the item you want </param>
        /// <param name="returnToStock"> Return the item to the stock by adding to the resources </param>
        /// <returns> Returns all the stored items </returns>
        public List<ItemInstance> GetAllResources(bool returnToStock = true)
        {
            List<ItemInstance> oldItemList = itemList;

            if (returnToStock)
            {
                foreach (ItemInstance item in this.itemList)
                {
                    NpcManager.Instance.RemoveAvailableResource(item.Resource.ResourceType, 1);
                    item.Resource.Amount++;
                }
            }
            ResetStorage();

            return oldItemList;
        }

        private void ResetStorage()
        {
            itemPhysicalHolderArray = new ItemInstance[maxResources];
            this.itemList.Clear();
            InvokeChange();
        }

        // Update the text of the UI
        protected override void UpdateUI()
        {
            GridUIManager.Instance.SetInteractableUI(name, description, InteractableUIElement.Storage, this, GetArgsForUI);
        }

        public bool HasResource(ResourceType type)
        {
            for (int i = 0; i < GetItemCount; i++)
            {
                if (type == itemList[i].Resource.ResourceType) return true;
            }
            return false;
        }

        #endregion Inventory Management

        #region Physical Storage

        // Add mesh to the physical object array (search for first one empty)
        private void AddPhysicalMesh(ItemInstance item)
        {
            int indexInPhysicalStorage = itemPhysicalHolderArray.ToList().FindIndex((obj) => obj == null);
            itemPhysicalHolderArray[indexInPhysicalStorage] = item;
        }

        // Remove mesh from the physical object array
        private void RemovePhysicalMesh(ItemInstance item)
        {
            int indexInPhysicalStorage = itemPhysicalHolderArray.ToList().FindIndex((obj) => obj == item);
            itemPhysicalHolderArray[indexInPhysicalStorage] = null;
        }

        // Update the meshes on the grid
        private void UpdateVisuals()
        {
            for (int i = 0; i < itemPhysicalHolderArray.Length; i++)
            {
                itemHolderMeshList[i].mesh = itemPhysicalHolderArray[i]?.Resource.Mesh[0];
            }
        }

        #endregion Physical Storage
    }
}