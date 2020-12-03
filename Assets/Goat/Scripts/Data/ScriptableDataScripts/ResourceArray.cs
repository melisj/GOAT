﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Storage;
using Sirenix.OdinInspector;

namespace Goat.Storage
{
    /// <summary>
    /// Creates a list of resources inside the project.
    /// Can be used for customer to check what resources are available or not.
    /// </summary>
    [CreateAssetMenu(fileName = "ResourceArray", menuName = "ScriptableObjects/ResourceArray")]
    public class ResourceArray : ScriptableObject
    {
        [PropertyOrder(1), PropertySpace(10)]
        [SerializeField] private Resource[] resources;
        public Resource[] Resources => resources;

        private void OnValidate()
        {
            resources = UnityEngine.Resources.LoadAll<Resource>("");
        }

        [Button("Load Resources"), PropertyOrder(0)]
        void LoadResources()
        {
            resources = UnityEngine.Resources.LoadAll<Resource>("");
        }
    }
}
