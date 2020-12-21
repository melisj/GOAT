using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.ScriptableObjects;
using Goat.Grid.Interactions;
using Goat.AI.Parking;
using Goat.AI.Satisfaction;
using UnityAtoms.BaseAtoms;

namespace Goat.AI
{
    public class Worker : NPC
    {
        public StorageList storageLocations;
        [HideInInspector] public List<StorageInteractable> targetStorages = new List<StorageInteractable>();
    }
}