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

    [CreateAssetMenu(fileName = "FarmBuildingSettings", menuName = "ScriptableObjects/FarmBuildingSettings")]
    public class FarmBuildingSettings : SerializedScriptableObject
    {
        [SerializeField] private GameObject prefabBuilding;
        [SerializeField] private Sprite buildingImage;
        [SerializeField] private float price;
        [SerializeField] private float storageCapacity;
        [SerializeField] private ResourceCost[] buyCost;
        [SerializeField] private Resource resourceFarm;
        [SerializeField] private int amountPerSecond;
        [SerializeField, EnumToggleButtons()] private FarmType farmType;
        [SerializeField, EnumToggleButtons()] private FarmDeliverType farmDeliverType;

        [SerializeField, ShowIf("farmType", FarmType.OverTimeCost)] private int costPerSecond;

        public string Name => resourceFarm.ResourceType.ToString() + " Farm";
        public GameObject PrefabBuilding => prefabBuilding;
        public Sprite BuildingImage => buildingImage;
        public ResourceCost[] BuyCost => buyCost;
        public Resource ResourceFarm => resourceFarm;
        public int AmountPerSecond => amountPerSecond;
        public int CostPerSecond => costPerSecond;
        public FarmType FarmType => farmType;
        public float Price => price;
        public FarmDeliverType FarmDeliverType => farmDeliverType;
        public float StorageCapacity => storageCapacity;
    }

    [System.Serializable]
    public class ResourceCost
    {
        [SerializeField] private ResourceType costType;
        [SerializeField] private int amount;

        public ResourceType CostType => costType;
        public int Amount => amount;
    }

    //WERKhouding
    //Geef me de feedback
    //Wat wil ik eigenlijk leren..
    // Jasper:
    // Goede WERKHOUDING
    // JE BENT WEL GOED, JE PAKT DINGEN ZELF OP, VEEL KAARTEN GEDAAN
    // GOED INITIATIEF
    // CONCEPTING LEVER JE OOK GOED INPUT UIT JE ZELF

    //Timo
    //JE PAKT VEEL KAARTJES OP, VERDUIDELIJKEN WAT JE PRECIES GAAT DOEN EN WANNEER
    //KOMT MISS OOK DOOR GEEN STANDUPS
    //MEER OP INPUT VRAGEN

    //Kwaliteit
    //meer comments in code
    //Teamwork input

    //Bijdrage
    //Goed
}