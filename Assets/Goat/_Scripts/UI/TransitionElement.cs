using UnityEngine;
using Sirenix.OdinInspector;

namespace Goat.UI
{
    [System.Serializable]
    public class TransitionElement
    {
        [SerializeField] private TransitionType transType;
        [SerializeField] private RectTransform transform;
        [SerializeField, ShowIf("showScale")] private Vector3 beforeScale;
        [SerializeField, ShowIf("showMove")] private Vector3 moveAmount;

        private bool showScale => transType.HasFlag(TransitionType.Scale);
        private bool showMove => transType.HasFlag(TransitionType.Move);

        public RectTransform Transform => transform;
        public Vector3 MoveAmount => moveAmount;
        public Vector3 BeforeScale => beforeScale;
        public TransitionType TransType => transType;
    }
}