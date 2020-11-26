using Goat.Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Grid.Interactions
{
    public class ItemInstance
    {
        public ItemInstance(Resource type) {
            Resource = type;
        }

        public Resource Resource { get; private set; }
    }
}