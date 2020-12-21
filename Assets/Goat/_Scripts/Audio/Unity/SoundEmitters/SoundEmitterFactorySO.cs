using UnityEngine;
using Goat.Factory;
using Goat.Pooling;

[CreateAssetMenu(fileName = "NewSoundEmitterFactory", menuName = "Factory/SoundEmitter Factory")]
public class SoundEmitterFactorySO : FactorySO<SoundEmitter>
{
    [SerializeField] private GameObject prefab = default;

    public override SoundEmitter Create()
    {
        GameObject obj = PoolManager.Instance.GetFromPool(prefab.gameObject);
        SoundEmitter emitter = obj.AddComponent<SoundEmitter>();
        return emitter;
    }
}