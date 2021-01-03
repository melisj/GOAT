using UnityEngine;
using Goat.Pooling;

namespace Goat.Storage
{
    public class ResourcePack : MonoBehaviour, IPoolObject
    {
        [SerializeField] private Buyable buyable;
        [SerializeField] private float amount;

        [SerializeField] private MeshFilter filter;

        public Resource Resource => (Resource)buyable;

        public float Amount { get => amount; set => amount = value; }

        public void SetupResPack(Buyable buyable, int amount)
        {
            this.amount = amount;
            this.buyable = buyable;
            filter.mesh = this.buyable.Mesh[0];
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Storage"))
            {
                buyable.Amount += (int)amount;
                amount = 0;
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
            PoolManager.Instance.SetParent(gameObject);
        }

        public int PoolKey { get; set; }
        public ObjectInstance ObjInstance { get; set; }
        public float Amount { get => amount; set => amount = value; }
    }

    
}