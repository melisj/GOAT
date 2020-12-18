using UnityEngine;
using Goat.Pooling;

public class PoolExample : MonoBehaviour, IPoolObject
{
    public int PoolKey { get; set; }
    public ObjectInstance ObjInstance { get; set; }

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