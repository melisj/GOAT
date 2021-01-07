using UnityEngine;
using Goat.Factory;
using Goat.Pooling;

[CreateAssetMenu(fileName = "NewSoundEmitterFactory", menuName = "Factory/SoundEmitter Factory")]
public class SoundEmitterFactorySO : FactorySO<SoundEmitter>
{
    [SerializeField] private GameObject prefab = default;
    [SerializeField] private bool usePooling;

    public override SoundEmitter Create(Vector3 pos, Transform parent = default)
    {
        GameObject obj = PoolManager.Instance.GetFromPool(prefab.gameObject, pos, Quaternion.identity, parent);

        SoundEmitter emitter = obj.GetComponent<SoundEmitter>();
        return emitter;
    }
}