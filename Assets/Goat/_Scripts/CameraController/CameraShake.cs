using Cinemachine;
using DG.Tweening;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Goat.CameraControls
{
    public class CameraShake : MonoBehaviour, IAtomListener<UnityAtoms.Void>
    {
        [SerializeField] private VoidEvent onMeteorImpact;
        [SerializeField] private CinemachineVirtualCamera vCam;
        [SerializeField] private float ampGain, frequencyGain;
        [SerializeField] private float duration;
        private CinemachineFramingTransposer transposer;
        private CinemachineBasicMultiChannelPerlin noise;
        private Sequence shakeSequence;

        private void Awake()
        {
            noise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        private void OnEnable()
        {
            onMeteorImpact.RegisterSafe(this);
        }

        private void OnDisable()
        {
            onMeteorImpact.UnregisterSafe(this);
        }

        public void OnEventRaised(Void item)
        {
            if (shakeSequence.NotNull())
                shakeSequence.Complete();
            shakeSequence = DOTween.Sequence();
            shakeSequence.SetUpdate(true);
            shakeSequence.Append(DOTween.To(() => 0, x => noise.m_AmplitudeGain = x, ampGain, duration / 4));
            shakeSequence.Join(DOTween.To(() => 0, x => noise.m_FrequencyGain = x, frequencyGain, duration / 4));
            shakeSequence.Append(DOTween.To(() => ampGain, x => noise.m_AmplitudeGain = x, 0, duration / 4));
            shakeSequence.Join(DOTween.To(() => frequencyGain, x => noise.m_FrequencyGain = x, 0, duration / 4));
        }
    }
}