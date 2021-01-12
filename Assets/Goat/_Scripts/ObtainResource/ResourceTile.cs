using Goat.Helper;
using Goat.Pooling;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Goat.Grid
{
    public class ResourceTile : MonoBehaviour, IPoolObject
    {
        [SerializeField] private ResourceTileData data;
        [SerializeField] private int amount;
        [SerializeField] private MeshFilter filter;
        [SerializeField] private MaterialPropertySetter propSetter;
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

        public void Setup(ResourceTileData data)
        {
            this.data = data;
            amount = Data.StarterAmount;
            propSetter.MaterialValueToChanges[0].NewFloat = data.HueShift;
            propSetter.ModifyValues();
        }

        private void OnResourceDepleted()
        {
            if (amount <= 0)
            {
                PoolManager.Instance.ReturnToPool(gameObject);
            }
        }

        public void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            ObjInstance = objectInstance;
            PoolKey = poolKey;
        }

        public void OnReturnObject()
        {
            gameObject.SetActive(false);
        }
    }
}