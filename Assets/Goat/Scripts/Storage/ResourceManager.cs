using System.Collections.Generic;
using UnityEngine;

namespace Goat.Resource
{
    public class ResourceManager : MonoBehaviour
    {
        private ResourceDictionary resData;

        public Resource GetResourceInfo(ResourceType type)
        {
            Resource resource = null;
            if (!resData.Resources.ContainsKey(type) || resData.Resources.TryGetValue(type, out resource)) 
            {
                Debug.LogErrorFormat("Resource {0} could not be found in dictionary", type);
                return resource;
            } 
            return resource;
        }

        public void ChangeResourceAmount(ResourceType type, int amount = 1)
        {
            Resource resource = null;
            if (!resData.Resources.ContainsKey(type) || resData.Resources.TryGetValue(type, out resource)) return;
            resource.Amount += amount;
        }
    }
}