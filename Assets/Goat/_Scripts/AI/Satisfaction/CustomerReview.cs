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

        private void Awake()
        {
            reviewFactors = GetComponentsInChildren<ReviewFactor>();
        }

        [Button]
        public void WriteReview()
        {
            Review review = new Review();

            review.ReviewFactors = new int[reviewFactors.Length];

            for (int i = 0; i < reviewFactors.Length; i++)
            {
                review.ReviewFactors[i] = (int)reviewFactors[i].GetReviewPoints();
                review.SatisfactionPoints += reviewFactors[i].GetReviewPoints();
            }

            review.SatisfactionPoints /= reviewFactors.Length;

            reviewEvent.Raise(review);
        }
    }
}