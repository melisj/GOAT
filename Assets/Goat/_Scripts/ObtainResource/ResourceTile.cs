using Goat.Pooling;
using UnityEngine;

namespace Goat.Grid
{
    public class ResourceTile : MonoBehaviour, IPoolObject
    {
        [SerializeField] private ResourceTileData data;
        [SerializeField] private int amount;
        [SerializeField] private MeshFilter filter;

        public int PoolKey { get; set; }
        public ObjectInstance ObjInstance { get; set; }
        public ResourceTileData Data => data;

        public int Amount
        {
            get => amount;
            set
            {
                if (amount <= 0 && value <= 0)
                {
                    amount = 0;
                }
                else
                {
                    amount = value;
                }
                OnResourceDepleted();
            }
        }

        private void Setup()
        {
            filter.mesh = Data.ResourceTile;
            amount = Data.StarterAmount;
        }

        private void OnResourceDepleted()
        {
            if (amount <= 0)
            {
                filter.mesh = Data.DefaultTile;
            }
        }

        public void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            Setup();
            ObjInstance = objectInstance;
            PoolKey = poolKey;
        }

        public void OnReturnObject()
        {
            gameObject.SetActive(false);
        }
    }
}