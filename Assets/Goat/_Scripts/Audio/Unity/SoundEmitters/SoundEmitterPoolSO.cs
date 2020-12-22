using UnityEngine;
using System.Linq;
using Goat.Factory;

[CreateAssetMenu(fileName = "NewSoundEmitterPool", menuName = "Pool/SoundEmitter Pool")]
public class SoundEmitterPoolSO : FactorySO<SoundEmitter>
{
    [SerializeField]
    private SoundEmitterFactorySO _factory;

    public override SoundEmitter Create()
    {
        throw new System.NotImplementedException();
    }
}