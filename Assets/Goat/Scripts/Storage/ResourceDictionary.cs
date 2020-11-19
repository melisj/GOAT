using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Goat.Resource
{
    [CreateAssetMenu(fileName = "ResourceDictionary", menuName = "ScriptableObjects/ResourceDictionary")]
    public class ResourceDictionary : SerializedScriptableObject
    {
        [SerializeField] private Dictionary<ResourceType, Resource> resources;
        public Dictionary<ResourceType, Resource> Resources => resources;
    }
}