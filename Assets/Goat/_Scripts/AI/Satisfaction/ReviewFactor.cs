using UnityEngine;
using Sirenix.OdinInspector;

namespace Goat.AI.Satisfaction
{
    public abstract class ReviewFactor : MonoBehaviour, IReviewFactor
    {
        [SerializeField, InlineEditor] protected ReviewWeight reviewWeight;
        [SerializeField, InlineEditor] protected ReviewLines reviewLines;
        [SerializeField, InlineEditor] protected ReviewFactorType factorType;
        public ReviewLines ReviewLines => reviewLines;

        public ReviewFactorType FactorType => factorType;

        public abstract float GetReviewPoints();
    }
}