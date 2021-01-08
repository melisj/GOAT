using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Goat.Storage;

namespace Goat.Farming
{
    public enum FarmType
    {
        OverTimeCost,
        NoCost
    }

    public enum FarmDeliverType
    {
        AutoContinuously,
        AutoWhenFull,
        Self
    }

    [CreateAssetMenu(fileName = "FarmStation", menuName = "ScriptableObjects/Buyable/FarmStation")]
    public class FarmStation : Building
    {
        //[SerializeField, Space(10)] private Resource resourceFarm;
        [SerializeField, Space(10)] private Resource[] resourceFarms;
        [SerializeField, EnumToggleButtons()] private FarmType farmType;
        [SerializeField] private float storageCapacity = 1;
        [SerializeField] private int amountPerSecond = 2;
        [SerializeField] private int amountPerResource = 2;
        [SerializeField, ShowIf("farmType", FarmType.OverTimeCost)] private int costPerSecond;
        [SerializeField] private ResourceCost[] resourceCost;
        [SerializeField, EnumToggleButtons()] private FarmDeliverType farmDeliverType;

        public ResourceCost[] ResourceCost => resourceCost;

        //public Resource ResourceFarm => resourceFarm;
        public Resource[] ResourceFarms => resourceFarms;

        public int AmountPerSecond => amountPerSecond;
        public int CostPerSecond => costPerSecond;
        public FarmType FarmType => farmType;

        public FarmDeliverType FarmDeliverType => farmDeliverType;

        public float StorageCapacity => storageCapacity;

        public int AmountPerResource => amountPerResource;
    }

    [System.Serializable]
    public class ResourceCost
    {
        [SerializeField] private Resource resource;
        [SerializeField] private int amount;

        public Resource GetResource => resource;
        public int Amount => amount;
    }
}