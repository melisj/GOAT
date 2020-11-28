using Goat.Pooling;
using Goat.Storage;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Goat.Farming
{
    public class FarmStationFunction : MonoBehaviour
    {
        private const float delay = 1f;
        [SerializeField, InlineEditor, AssetList(Path = "/Goat/ScriptableObjects/Farming")] private FarmStation farmStationSettings;
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
            resPackObj.name = "ResourcePack-" + farmStationSettings.ResourceFarm.ResourceType.ToString();
            ResourcePack resPack = resPackObj.GetComponent<ResourcePack>();
            resPack.Resource = farmStationSettings.ResourceFarm;
            resPack.SetupResPack();
            int amount = capacity < currentCapacity ? capacity : currentCapacity;
            resPack.Amount = amount;
            currentCapacity -= amount;
        }

        private void AddResource()
        {
            if (currentCapacity >= farmStationSettings.StorageCapacity)
            {
                timer = 0;
                return;
            }
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                timer = 0;
                currentCapacity += farmStationSettings.AmountPerSecond;
                //GetAsteroidInfo.Capacity(farmBuildingSettings.ResourceFarm.ResourceType) -= farmBuildingSettings.AmountPerSecond;

                if (farmStationSettings.FarmDeliverType == FarmDeliverType.AutoContinuously)
                {
                    //&& isConnected
                    currentCapacity -= farmStationSettings.AmountPerSecond;
                    farmStationSettings.ResourceFarm.Amount += farmStationSettings.AmountPerSecond;
                }

                if (farmStationSettings.FarmType == FarmType.OverTimeCost)
                {
                    farmStationSettings.ResourceFarm.Money.Amount -= farmStationSettings.CostPerSecond;
                    //GameManager.Instance.Money -= farmBuildingSettings.CostPerSecond;
                }
            }
        }
    }
}