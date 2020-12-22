using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.ScriptableObjects;
using Goat.Grid.Interactions;

namespace Goat.Pooling
{
    public class StorageListManager : MonoBehaviour
    {
        [SerializeField] private StorageList storages;
        private StorageInteractable storage;

        public int PoolKey { get; set; }

        public ObjectInstance ObjInstance { get; set; }

        private void Awake()
        {
            storage = GetComponent<StorageInteractable>();
        }

        private void OnEnable()
        {
            storages.Add(storage);
        }        
        private void OnDisable()
        {
            storages.Remove(storage);
        }

        private void OnDestroy()
        {
            storages.Remove(storage);
        }
    }
}

