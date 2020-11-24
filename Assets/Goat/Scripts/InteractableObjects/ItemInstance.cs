using Goat.Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAT
{
    public class ItemInstance : MonoBehaviour
    {
        [SerializeField] private Resource resource;

        public ItemInstance(Resource type) {
            resource = type;
        }

        public Resource GetResource => resource;
    }
}