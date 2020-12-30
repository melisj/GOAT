using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.AI.Satisfaction
{
    public class CustomerReview : MonoBehaviour
    {
        [SerializeField, ReadOnly] private ReviewFactor[] reviewFactors;
        [SerializeField] private ReviewEvent reviewEvent;
        [SerializeField] private GameObject root;

        private void Awake()
        {
            reviewFactors = GetComponentsInChildren<ReviewFactor>();
        }

        [Button]
        public void WriteReview()
        {
            Review review = new Review
            {
                ReviewFactors = new ReviewLineInfo[reviewFactors.Length],
                SatisfactionPoints = 0,
                CustomerName = root.name
            };

            for (int i = 0; i < reviewFactors.Length; i++)
            {
                ReviewFactor revFactor = reviewFactors[i];
                int revPoint = (int)revFactor.GetReviewPoints();
                string line = revPoint > 0 ? revFactor.ReviewLines.GetPositiveLine : revFactor.ReviewLines.GetNegativeLine;

                review.ReviewFactors[i] = new ReviewLineInfo(line, revPoint, revFactor.FactorType);
                review.SatisfactionPoints += revPoint;
            }

            Debug.Log($"{review.SatisfactionPoints} {reviewFactors.Length} {review.SatisfactionPoints / reviewFactors.Length}");
            review.SatisfactionPoints /= reviewFactors.Length;

            reviewEvent.Raise(review);
        }
    }
}