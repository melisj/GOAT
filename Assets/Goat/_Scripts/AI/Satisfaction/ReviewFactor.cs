using UnityEngine;
using Sirenix.OdinInspector;

namespace Goat.AI.Satisfaction
{
    public abstract class ReviewFactor : MonoBehaviour, IReviewFactor
    {
        [SerializeField, InlineEditor] protected ReviewData revData;
        [SerializeField, EnumToggleButtons] protected ReviewFactorType factorType;
        public ReviewData ReviewData => revData;

        public ReviewFactorType FactorType => factorType;

        public abstract float GetReviewPoints();
    }
}