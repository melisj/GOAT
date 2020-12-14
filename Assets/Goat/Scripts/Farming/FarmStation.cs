﻿using System.Collections;
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

    [CreateAssetMenu(fileName = "FarmStation", menuName = "ScriptableObjects/FarmStation")]
    public class FarmStation : Building
    {
        [SerializeField, Space(10)] private Resource resourceFarm;
        [SerializeField, EnumToggleButtons()] private FarmType farmType;
        [SerializeField] private float storageCapacity = 1;
        [SerializeField] private int amountPerSecond = 2;
        [SerializeField] private int amountPerResource = 2;
        [SerializeField, ShowIf("farmType", FarmType.OverTimeCost)] private int costPerSecond;
        [SerializeField] private ResourceCost[] resourceCost;
        [SerializeField, EnumToggleButtons()] private FarmDeliverType farmDeliverType;

        public string Name => resourceFarm.ResourceType.ToString() + " Farm";

        public ResourceCost[] ResourceCost => resourceCost;

        public Resource ResourceFarm => resourceFarm;
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
        [SerializeField] private ResourceType costType;
        [SerializeField] private int amount;

        public ResourceType CostType => costType;
        public int Amount => amount;
    }
}