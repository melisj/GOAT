﻿using System.Collections;
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
    [CreateAssetMenu(fileName = "StorageList", menuName = "ScriptableObjects/RuntimeVariables/StorageList")]
    public class StorageList : ScriptableObject
    {
        [SerializeField] private List<StorageInteractable> storageTransformLocation = new List<StorageInteractable>();
        public List<StorageInteractable> Storages => storageTransformLocation;
        public void Add(StorageInteractable storage)
        {
            storageTransformLocation.Add(storage);
        }
        public void Remove(StorageInteractable storage)
        {
            storageTransformLocation.Remove(storage);
        }
    }
}

