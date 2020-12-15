using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.ScriptableObjects;
using Goat.Grid.Interactions;

namespace Goat.Pooling
{
    public class StoragePool : MonoBehaviour, IPoolObject
    {
        [SerializeField] private StorageLocations storages;
        [SerializeField] private StorageInteractable storage;

        public int PoolKey { get; set; }

        public ObjectInstance ObjInstance { get; set; }

        private void Awake()
        {
            storage = GetComponent<StorageInteractable>();
        }

        public void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            storages.AddStorage(storage);
            ObjInstance = objectInstance;
            PoolKey = poolKey;
        }

        public void OnReturnObject()
        {
            storages.RemoveStorage(storage);

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            storages.RemoveStorage(storage);
        }
    }
}

