using Goat.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpDriveSequence : MonoBehaviour, IPoolObject
{
    public int PoolKey { get; set; }
    public ObjectInstance ObjInstance { get; set; }

    [SerializeField] private int sequenceLength;

    public void OnGetObject(ObjectInstance objectInstance, int poolKey)
    {
        PoolKey = poolKey;
        ObjInstance = objectInstance;
        StartCoroutine(WarpSequence());
    }

    public void OnReturnObject()
    {
        gameObject.SetActive(false);
    }
    
    private IEnumerator WarpSequence()
    {
        yield return new WaitForSeconds(sequenceLength);

        PoolManager.Instance.ReturnToPool(gameObject);
    }
}
