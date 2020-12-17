using Goat.Pooling;
using Goat.Storage;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Goat.Grid.Interactions
{
    public class ShelfInteractable : StorageInteractable
    {
        public List<MeshFilter> itemHolderMeshList = new List<MeshFilter>();
        public Resource[] itemResourceArray;

        // Get or create a item holder object
        private Transform ItemHolderParent;
        private Transform GetItemHolderParent
        {
            get
            {
                ItemHolderParent = transform.Find(info.ItemHolderParentName) ??
                    new GameObject(info.ItemHolderParentName).transform;

                if (ItemHolderParent.parent != transform)
                    ItemHolderParent.SetParent(transform, false);

                return ItemHolderParent;
            }
        }

        public override object[] GetArgumentsForUI()
        {
            return new object[] {
            string.Format("Storage -=- {0}/{1}", Inventory.ItemsInInventory, maxResources),
            Inventory,
            this };
        }

        protected override void Awake()
        {
            base.Awake();
            Inventory.InventoryResetEvent += Inventory_InventoryResetEvent;
            InitStorage();
            ResetVisuals();
        }

        private void Inventory_InventoryResetEvent()
        {
            ResetVisuals();
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
            itemHolder.GetComponent<MeshRenderer>().material = info.ItemMaterial;
            itemHolderMeshList.Add(itemHolder.GetComponent<MeshFilter>());
            itemHolderMeshList[itemHolderMeshList.Count - 1].gameObject.SetActive(false);
        }

        // Create a new item holder
        private GameObject CreateItemHolder()
        {
            GameObject itemHolder = new GameObject(info.ItemHolderName, typeof(MeshFilter), typeof(MeshRenderer));
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

        #endregion

        #region Physical Storage

        // Update the meshes on the grid
        private void ResetVisuals()
        {
            itemResourceArray = new Resource[maxResources];

            for (int i = Inventory.ItemsInInventory; i < itemResourceArray.Length; i++)
            {
                itemHolderMeshList[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < Inventory.Items.Count; i++)
            {
                for (int j = 0; j < Inventory.Items.ElementAt(i).Value; j++)
                {
                    int index = itemResourceArray.ToList().FindIndex(x => x == null);
                    itemResourceArray[index] = Inventory.Items.ElementAt(i).Key;

                    itemHolderMeshList[index].gameObject.SetActive(true);
                    itemHolderMeshList[index].mesh = itemResourceArray[index].Mesh[0];
                }
            }
        }

        public override bool Add(Resource resource, int amount, out int storedAmount)
        {
            bool succeeded = base.Add(resource, amount, out storedAmount);
            for (int i = 0; i < storedAmount; i++)
            {
                int index = itemResourceArray.ToList().FindIndex(x => x == null);
                itemResourceArray[index] = resource;

                itemHolderMeshList[index].mesh = resource.Mesh[0];
                itemHolderMeshList[index].gameObject.SetActive(true);
            }

            return succeeded;
        }

        public override void Remove(Resource resource, int amount, out int removedAmount)
        {
            base.Remove(resource, amount, out removedAmount);
            for (int i = 0; i < removedAmount; i++)
            {
                int index = itemResourceArray.ToList().FindIndex(x => x == resource);
                itemResourceArray[index] = null;

                itemHolderMeshList[index].gameObject.SetActive(false);
            }
        }

        #endregion

        public override void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            base.OnGetObject(objectInstance, poolKey);
        }

        public override void OnReturnObject()
        {
            Inventory.Clear();

            base.OnReturnObject();
        }
    }
}