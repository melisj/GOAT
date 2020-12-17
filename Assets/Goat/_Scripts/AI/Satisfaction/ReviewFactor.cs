using UnityEngine;
using Sirenix.OdinInspector;

namespace Goat.AI.Satisfaction
{
    public abstract class ReviewFactor : MonoBehaviour, IReviewFactor
    {
        [SerializeField, InlineEditor] protected ReviewWeight reviewWeight;

        public abstract float GetReviewPoints();
    }
}