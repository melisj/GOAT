using UnityEngine;

namespace Goat.AI.Satisfaction
{
    public abstract class ReviewFactor : MonoBehaviour, IReviewFactor
    {
        [SerializeField] protected ReviewWeight reviewWeight;

        public abstract float GetReviewPoints();
    }
}