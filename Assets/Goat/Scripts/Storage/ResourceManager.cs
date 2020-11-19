using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Resource
{
    public class ResourceManager : MonoBehaviour
    {
        [SerializeField] private ResourceDictionary resData;

        public Resource GetResourceInfo(ResourceType type)
        {
            Resource resource = null;
            if (!resData.Resources.ContainsKey(type) || !resData.Resources.TryGetValue(type, out resource)) 
            {
                Debug.LogErrorFormat("Resource {0} could not be found in dictionary", type);
                return resource;
            } 
            return resource;
        }
        [Button]
        public void ChangeResourceAmount([EnumToggleButtons()]ResourceType type, int amount = 1)
        {
            Resource resource = null;
            if (!resData.Resources.ContainsKey(type) || !resData.Resources.TryGetValue(type, out resource)) return;
            resource.Amount += amount;
        }
    }
}