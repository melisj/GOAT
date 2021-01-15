using UnityEngine;
using Goat.Pooling;
using System;

namespace Goat.Storage
{
    public class ResourcePack : MonoBehaviour, IPoolObject
    {
        [SerializeField] private Buyable buyable;
        //[SerializeField] private float amount;

        [SerializeField] private MeshFilter filter;

        public Resource Resource => (Resource)buyable;
        //public ResourcePackInfo PackInfo => new ResourcePackInfo(Resource.ID, Amount, transform.position);

        //public float Amount { get => amount; set => amount = value; }

        public ResourcePackInfo ResourcePackInfo => new ResourcePackInfo(buyable.ID, transform.position, transform.rotation.eulerAngles);

        public void SetupResPack(Buyable buyable)
        {
            //this.amount = amount;
            this.buyable = buyable;
            filter.mesh = this.buyable.Mesh[0];
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
    }

    [Serializable]
    public class ResourcePackInfo
    {
        public int resource;
        public Vector3 position;
        public Vector3 rotation;

        public ResourcePackInfo(int resource, Vector3 position, Vector3 rotation)
        {
            this.resource = resource;
            this.position = position;
            this.rotation = rotation;
        }
    }
}