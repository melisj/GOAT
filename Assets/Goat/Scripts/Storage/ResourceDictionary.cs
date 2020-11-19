using System.Collections.Generic;
using UnityEngine;

namespace Goat.Resource
{
    [CreateAssetMenu(fileName = "ResourceDictionary", menuName = "ScriptableObjects/ResourceDictionary")]
    public class ResourceDictionary : ScriptableObject
    {
        [SerializeField] private Dictionary<ResourceType, Resource> resources;
        public Dictionary<ResourceType, Resource> Resources => resources;
    }
}