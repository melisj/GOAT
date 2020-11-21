using UnityEngine;

namespace Goat.Pooling
{
    public interface IPoolObject
    {
        int PoolKey { get; set; }
        ObjectInstance ObjInstance { get; set; }

        void OnGetObject(ObjectInstance objectInstance, int poolKey);

        void OnReturnObject();
    }
}