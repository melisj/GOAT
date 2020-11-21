using Goat.Storage;
using Goat.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Farming
{
    public class FarmBuilding : MonoBehaviour
    {
        private const float delay = 1f;
        [SerializeField] private FarmBuildingSettings farmBuildingSettings;
        [SerializeField] private GameObject resPackPrefab;
        [SerializeField] private int currentCapacity;
        private float timer;
        private bool isConnected;

        private void Update()
        {
            AddResource();
        }

        public void CreateResourcePack(int capacity, Transform parent)
        {
            GameObject resPackObj = PoolManager.Instance.GetFromPool(resPackPrefab, Vector3.zero, Quaternion.identity, parent);
            resPackObj.name = "ResourcePack-" + farmBuildingSettings.ResourceFarm.ResourceType.ToString();
            ResourcePack resPack = resPackObj.GetComponent<ResourcePack>();
            resPack.Resource = farmBuildingSettings.ResourceFarm;
            resPack.SetupResPack();
            int amount = capacity < currentCapacity ? capacity : currentCapacity;
            resPack.Amount = amount;
            currentCapacity -= amount;
        }

        private void AddResource()
        {
            if (currentCapacity < farmBuildingSettings.StorageCapacity)
            {
                timer = 0;
                return;
            }
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                timer = 0;
                currentCapacity += farmBuildingSettings.AmountPerSecond;
                //GetAsteroidInfo.Capacity(farmBuildingSettings.ResourceFarm.ResourceType) -= farmBuildingSettings.AmountPerSecond;
                if (farmBuildingSettings.FarmType == FarmType.OverTimeCost)
                {
                    //GameManager.Instance.Money -= farmBuildingSettings.CostPerSecond;
                }
            }
        }
    }
}