using Goat.Grid;
using Goat.Pooling;
using Goat.Storage;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Goat.Farming
{
    public class FarmStationFunction : MonoBehaviour, IPoolObject
    {
        private const float delay = 1f;
        [SerializeField, InlineEditor, AssetList(Path = "/Goat/ScriptableObjects/Farming")] private FarmStation farmStationSettings;
        [SerializeField] private float radius = 1;
        [SerializeField] private GameObject resPackPrefab;
        [SerializeField] private int currentCapacity;
        [SerializeField] private LayerMask floorLayer;
        [SerializeField] private Animator animator;
        private float timer;
        private bool isConnected;
        private ResourceTile resourceTile;

        public int PoolKey { get; set; }
        public ObjectInstance ObjInstance { get; set; }

        private void Update()
        {
            AddResource();
        }

        public void CreateResourcePack(int capacity, Transform parent)
        {
            GameObject resPackObj = PoolManager.Instance.GetFromPool(resPackPrefab, Vector3.zero, Quaternion.identity, parent);
            resPackObj.name = "ResourcePack-" + farmStationSettings.ResourceFarm.ResourceType.ToString();
            ResourcePack resPack = resPackObj.GetComponent<ResourcePack>();
            int amount = capacity < currentCapacity ? capacity : currentCapacity;
            resPack.SetupResPack(farmStationSettings.ResourceFarm, amount);
            currentCapacity -= amount;
        }

        private void AddResource()
        {
            if (currentCapacity >= farmStationSettings.StorageCapacity || resourceTile.Amount <= 0)
            {
                animator.enabled = false;
                timer = 0;
                return;
            }
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                animator.enabled = true;
                timer = 0;
                currentCapacity += farmStationSettings.AmountPerSecond;
                resourceTile.Amount -= farmStationSettings.AmountPerSecond;
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

        public void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            Setup();
            ObjInstance = objectInstance;
            PoolKey = poolKey;
        }

        private void Setup()
        {
            GetResourceTile();
        }

        private void GetResourceTile()
        {
            resourceTile = null;
            Collider[] cols = Physics.OverlapSphere(transform.position, radius, floorLayer);
            if (cols.Length > 0)
            {
                resourceTile = cols[0].gameObject.GetComponent<ResourceTile>();
            }
        }

        public void OnReturnObject()
        {
            gameObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}