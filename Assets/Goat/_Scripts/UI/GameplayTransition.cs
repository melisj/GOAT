using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityAtoms;
using UnityAtoms.BaseAtoms;

[System.Flags]
public enum TransitionType
{
    None = 0,
    Scale = 1,
    Move = 2,
    All = 3
}

namespace Goat.UI
{
    public class GameplayTransition : MonoBehaviour, IAtomListener<UnityAtoms.Void>
    {
        private const float defaultResolution = 1920;

        [SerializeField] private TransitionElement[] transitionElements;
        [SerializeField] private Canvas inGameMenuElement;
        [SerializeField] private VoidEvent onNarrativeFinished;
        [SerializeField] private float transitionDurationPerElement;
        private Sequence transitionSequence;
        private bool prepared;
        private bool transitioned;
        private float resolution;

        private void OnEnable()
        {
            transitioned = false;
            onNarrativeFinished.RegisterSafe(this);
        }

        private void OnDisable()
        {
            onNarrativeFinished.UnregisterSafe(this);
        }

        public void Transition()
        {
            if (transitionSequence.NotNull())
                transitionSequence.Complete();
            resolution = Screen.width;
            float scaleFactor = (resolution / defaultResolution);
            transitionSequence = DOTween.Sequence();
            transitionSequence.SetUpdate(UpdateType.Normal, true);
            for (int i = 0; i < transitionElements.Length; i++)
            {
                TransitionElement element = transitionElements[i];
                Vector3 pos = element.Transform.position + (transitioned ? -element.MoveAmount * scaleFactor : element.MoveAmount * scaleFactor);
                Vector3 scale = (transitioned ? element.BeforeScale : Vector3.one);
                if (element.TransType.HasFlag(TransitionType.Move))
                    transitionSequence.Join(element.Transform.DOMove(pos, transitionDurationPerElement));
                if (element.TransType.HasFlag(TransitionType.Scale))
                    transitionSequence.Join(element.Transform.DOScale(scale, transitionDurationPerElement));
            }
            transitioned = !transitioned;
        }

        [ButtonGroup("")]
        private void PrepareTransition()
        {
            if (prepared)
            {
                return;
            }

            prepared = true;

            for (int i = 0; i < transitionElements.Length; i++)
            {
                TransitionElement element = transitionElements[i];

                if (element.TransType.HasFlag(TransitionType.Move))
                    element.Transform.position += element.MoveAmount;
                if (element.TransType.HasFlag(TransitionType.Scale))
                    element.Transform.localScale = element.BeforeScale;
            }
        }

        [ButtonGroup("")]
        private void RevertBack()
        {
            if (!prepared)
            {
                return;
            }
            prepared = false;
            for (int i = 0; i < transitionElements.Length; i++)
            {
                TransitionElement element = transitionElements[i];

                if (element.TransType.HasFlag(TransitionType.Move))
                    element.Transform.position -= element.MoveAmount;
                if (element.TransType.HasFlag(TransitionType.Scale))
                    element.Transform.localScale = Vector3.one;
            }
        }

        public void OnEventRaised(Void onNarrativeFinished)
        {
            if (transitioned || inGameMenuElement.enabled) return;
            Transition();
        }
    }
}