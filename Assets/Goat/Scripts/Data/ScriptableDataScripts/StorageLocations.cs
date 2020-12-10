using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Goat.Grid.Interactions
{
    /// <summary>
    /// Creates a list of StorageInteractable locations inside the project.
    /// Can be used for bpc to check where StorageInteractables are located.
    /// </summary>
    [CreateAssetMenu(fileName = "StorageLocations", menuName = "ScriptableObjects/StorageLocations")]
    public class StorageLocations : ScriptableObject
    {
        private List<Transform> storageTransformLocation = new List<Transform>();
        private StorageInteractable[] storageTransformArray;
        public List<Transform> StorageTransforms => storageTransformLocation;

        private void OnValidate()
        {
            FindStorageTransforms();
        }

        [Button("Find Storages")]
        public void FindStorageTransforms()
        {
            storageTransformLocation.Clear();
            storageTransformArray = Object.FindObjectsOfType<StorageInteractable>();

            for (int i = 0; i < storageTransformArray.Length; i++)
                storageTransformLocation.Add(storageTransformArray[i].transform);
        }

        public void AddStorage(Transform transform)
        {
            storageTransformLocation.Add(transform);
        }
        public void RemoveStorage(Transform transform)
        {
            storageTransformLocation.Remove(transform);
        }
    }
}

