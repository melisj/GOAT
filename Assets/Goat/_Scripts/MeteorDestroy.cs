using Goat.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorDestroy : MonoBehaviour, IPoolObject
{
    public int PoolKey { get; set; }
    public ObjectInstance ObjInstance { get; set; }
    [SerializeField] private AudioCue cue;

    public void OnGetObject(ObjectInstance objectInstance, int poolKey)
    {
        ObjInstance = objectInstance;
        PoolKey = poolKey;
        cue.PlayAudioCue(() => PoolManager.Instance.ReturnToPool(gameObject));
    }

    public void OnReturnObject()
    {
        gameObject.SetActive(false);
    }
}