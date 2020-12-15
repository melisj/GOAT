using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Goat.Grid.Interactions;

namespace Goat.ScriptableObjects
{
    /// <summary>
    /// Creates a list of StorageInteractable locations inside the project.
    /// Can be used for bpc to check where StorageInteractables are located.
    /// </summary>
    [CreateAssetMenu(fileName = "StorageLocations", menuName = "ScriptableObjects/RuntimeVariables/StorageLocations")]
    public class StorageLocations : ScriptableObject
    {
        private List<StorageInteractable> storageTransformLocation = new List<StorageInteractable>();
        private StorageInteractable[] storageTransformArray;
        public List<StorageInteractable> Storages => storageTransformLocation;

        //private void OnValidate()
        //{
        //    FindStorageTransforms();
        //}

        //[Button("Find Storages")]
        //public void FindStorageTransforms()
        //{
        //    storageTransformLocation.Clear();
        //    storageTransformArray = Object.FindObjectsOfType<StorageInteractable>();

        //    for (int i = 0; i < storageTransformArray.Length; i++)
        //        storageTransformLocation.Add(storageTransformArray[i].transform);
        //}

        public void AddStorage(StorageInteractable storage)
        {
            storageTransformLocation.Add(storage);
        }
        public void RemoveStorage(StorageInteractable storage)
        {
            storageTransformLocation.Remove(storage);
        }
    }
}

