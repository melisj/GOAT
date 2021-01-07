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
        [SerializeField] private TransitionElement[] transitionElements;
        [SerializeField] private VoidEvent onNarrativeFinished;
        [SerializeField] private float transitionDurationPerElement;
        private Sequence transitionSequence;
        private bool prepared;

        private void OnEnable()
        {
            onNarrativeFinished.RegisterSafe(this);
        }

        private void OnDisable()
        {
            onNarrativeFinished.UnregisterSafe(this);
        }

        private void Transition()
        {
            if (transitionSequence.NotNull())
                transitionSequence.Complete();

            transitionSequence = DOTween.Sequence();

            for (int i = 0; i < transitionElements.Length; i++)
            {
                TransitionElement element = transitionElements[i];

                if (element.TransType.HasFlag(TransitionType.Move))
                    transitionSequence.Join(element.Transform.DOMove(element.Transform.position + element.MoveAmount, transitionDurationPerElement));
                if (element.TransType.HasFlag(TransitionType.Scale))
                    transitionSequence.Join(element.Transform.DOScale(Vector3.one, transitionDurationPerElement));
            }
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
                    element.Transform.position -= element.MoveAmount;
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
                    element.Transform.position += element.MoveAmount;
                if (element.TransType.HasFlag(TransitionType.Scale))
                    element.Transform.localScale = Vector3.one;
            }
        }

        public void OnEventRaised(Void onNarrativeFinished)
        {
            Transition();
        }
    }
}