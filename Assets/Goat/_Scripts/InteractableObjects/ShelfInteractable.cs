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
        private List<MeshFilter> itemHolderMeshList = new List<MeshFilter>();

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
            InitStorage();
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
        private void UpdateVisuals()
        {
            for(int i = Inventory.ItemsInInventory; i < itemHolderMeshList.Count; i++)
            {
                itemHolderMeshList[i].mesh = null;
            } 

            for (int i = 0, h = 0; i < Inventory.Items.Count; i++)
            {
                for (int j = 0; j < Inventory.Items.ElementAt(i).Value; j++, h++)
                {
                    itemHolderMeshList[h].mesh = Inventory.Items.ElementAt(i).Key.Mesh[0];
                }
            }
        }

        public override void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            base.OnGetObject(objectInstance, poolKey);

            UpdateInteractable.AddListener(UpdateVisuals);
            ResetStorage();
        }

        public override void OnReturnObject()
        {
            ResetStorage();

            base.OnReturnObject();
        }

        #endregion

    }
}