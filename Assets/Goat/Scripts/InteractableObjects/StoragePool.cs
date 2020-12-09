using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Grid.Interactions;

namespace Goat.Pooling
{
    public class StoragePool : MonoBehaviour, IPoolObject
    {
        [SerializeField] private StorageLocations storages;

        public int PoolKey { get; set; }

        public ObjectInstance ObjInstance { get; set; }

        public void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            storages.AddStorage(transform);
            ObjInstance = objectInstance;
            PoolKey = poolKey;
        }

        public void OnReturnObject()
        {
            storages.RemoveStorage(transform);

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            storages.RemoveStorage(transform);
        }
    }
}

