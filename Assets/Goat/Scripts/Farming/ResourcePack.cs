using UnityEngine;
using Goat.Pooling;

namespace Goat.Storage
{
    public class ResourcePack : MonoBehaviour, IPoolObject
    {
        [SerializeField] private Resource resource;
        [SerializeField] private int amount;

        //[SerializeField] private MeshFilter filter;
        public void SetupResPack()
        {
            //filter.Mesh = resource.Mesh;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Storage"))
            {
                resource.Amount += amount;
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

        public Resource Resource { get => resource; set => resource = value; }
        public int Amount { get => amount; set => amount = value; }
        public int PoolKey { get; set; }
        public ObjectInstance ObjInstance { get; set; }
    }
}