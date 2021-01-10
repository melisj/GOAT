using Goat.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorDestroy : MonoBehaviour, IPoolObject
{
    public int PoolKey { get; set; }
    public ObjectInstance ObjInstance { get; set; }

    void Start()
    {
        StartCoroutine(DestroyThis());
    }

    IEnumerator DestroyThis()
    {
        yield return new WaitForSeconds(3);
        PoolManager.Instance.ReturnToPool(gameObject);
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
