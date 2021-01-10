using Goat.Pooling;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Goat.Delivery
{
    public class LocationRelay : MonoBehaviour, IPoolObject
    {
        [SerializeField] private UnloadLocations unload;
        private TileAnimation tileAnimation;
        public int PoolKey { get; set; }
        public ObjectInstance ObjInstance { get; set; }

        public void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            if (!tileAnimation)
                tileAnimation = GetComponent<TileAnimation>();
            tileAnimation.Prepare();
            unload.Locations.Add(transform.position);
            ObjInstance = objectInstance;
            PoolKey = poolKey;
        }

        public void OnReturnObject()
        {
            unload.Locations.Remove(transform.position);
            tileAnimation.Destroy(() => gameObject.SetActive(false));
        }

        private void OnDestroy()
        {
            unload.Locations.Remove(transform.position);
        }
    }
}