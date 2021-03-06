﻿using Goat.Pooling;
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
        [Header("Shelf Settings")]
        [SerializeField, TabGroup("Storage")] private float resourceSize = 1;

        private List<MeshFilter> itemHolderMeshList = new List<MeshFilter>();
        private Resource[] itemResourceArray;

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
            string.Format("{0} / {1}", Inventory.ItemsInInventory, maxResources),
            Inventory,
            this };
        }

        protected override void Awake()
        {
            base.Awake();
            Inventory.InventoryResetEvent += Inventory_InventoryResetEvent;
            InitStorage();
            ResetVisuals();

            Inventory.InventoryChangedEvent += Inventory_InventoryChangedEvent;
        }

        private void Inventory_InventoryChangedEvent(Resource resource, int amount, bool removed)
        {
            if (removed) Remove(resource, amount);
            else Add(resource, amount);
        }

        private void Inventory_InventoryResetEvent()
        {
            ResetVisuals();
        }

        #region Item Holders

        /// <summary>
        /// Initialize the holder items for the objects
        /// </summary>
        [Button("Generate/Get Item Holders"), TabGroup("Storage")]
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

            MeshFilter filter = itemHolder.GetComponent<MeshFilter>();
            itemHolderMeshList.Add(filter);
            filter.gameObject.SetActive(false);
            filter.transform.localScale = Vector3.one * resourceSize;
        }

        // Create a new item holder
        private GameObject CreateItemHolder()
        {
            GameObject itemHolder = new GameObject(info.ItemHolderName, typeof(MeshFilter), typeof(MeshRenderer));
            itemHolder.transform.SetParent(GetItemHolderParent, false);
            return itemHolder;
        }

        // Render out locations where meshes will be placed from the inventory
        private void OnDrawGizmosSelected()
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

        public void Add(Resource resource, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                int index = itemResourceArray.ToList().FindIndex(x => x == null);
                itemResourceArray[index] = resource;

                itemHolderMeshList[index].mesh = resource.Mesh[0];
                itemHolderMeshList[index].gameObject.SetActive(true);
            }
        }

        public void Remove(Resource resource, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                int index = itemResourceArray.ToList().FindIndex(x => x == resource);
                itemResourceArray[index] = null;

                itemHolderMeshList[index].gameObject.SetActive(false);
            }
        }

        #endregion Physical Storage

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