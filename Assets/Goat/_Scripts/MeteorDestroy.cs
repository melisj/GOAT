using Goat.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class MeteorDestroy : MonoBehaviour, IPoolObject
{
    public int PoolKey { get; set; }
    public ObjectInstance ObjInstance { get; set; }
    [SerializeField] private AudioCue cue;
    [SerializeField] private VoidEvent onMeteorImpact;

    public void OnGetObject(ObjectInstance objectInstance, int poolKey)
    {
        ObjInstance = objectInstance;
        PoolKey = poolKey;
        onMeteorImpact.Raise();
        cue.PlayAudioCue(() => PoolManager.Instance.ReturnToPool(gameObject));
    }

    public void OnReturnObject()
    {
        gameObject.SetActive(false);
    }
}