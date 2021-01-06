using UnityEngine;
using System.Linq;
using Goat.Factory;

[CreateAssetMenu(fileName = "NewSoundEmitterPool", menuName = "Pool/SoundEmitter Pool")]
public class SoundEmitterPoolSO : FactorySO<SoundEmitter>
{
    [SerializeField]
    private SoundEmitterFactorySO _factory;

    public override SoundEmitter Create(Vector3 pos, GameObject parent)
    {
        throw new System.NotImplementedException();
    }
}