using UnityEngine;
using Goat.Factory;
using Goat.Pooling;

[CreateAssetMenu(fileName = "NewSoundEmitterFactory", menuName = "Factory/SoundEmitter Factory")]
public class SoundEmitterFactorySO : FactorySO<SoundEmitter>
{
    [SerializeField] private GameObject prefab = default;

    public override SoundEmitter Create(Vector3 pos, GameObject parent)
    {
        GameObject obj = PoolManager.Instance.GetFromPool(prefab.gameObject, pos, Quaternion.identity, parent.transform);
        SoundEmitter emitter = obj.GetComponent<SoundEmitter>();
        return emitter;
    }
}