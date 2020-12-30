using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Goat.AI.Satisfaction;
using System.Text;
using Sirenix.OdinInspector;

namespace Goat.UI
{
    public class ReviewCell : MonoBehaviour
    {
        [SerializeField] private ConjunctiveAdverbs adverbs;
        [SerializeField] private TextMeshProUGUI customerName;
        [SerializeField] private TextMeshProUGUI review;
        [Title("Factors")]
        [SerializeField] private TextMeshProUGUI beautyPoints;
        [SerializeField] private TextMeshProUGUI itemNotFoundPoints;
        [SerializeField] private TextMeshProUGUI searchTimePoints;

        public void Setup(Review review)
        {
            ReviewLineInfo prevInfo;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < review.ReviewFactors.Length; i++)
            {
                ReviewLineInfo revInfo = review.ReviewFactors[i];
                if (i > 0)
                {
                    prevInfo = review.ReviewFactors[i - 1];

                    stringBuilder.Append((prevInfo.Points < 0 ^ revInfo.Points < 0 ? adverbs.GetContrast : adverbs.GetAddition));
                }
                stringBuilder.Append(revInfo.Line);

                SetFactors(revInfo);
            }
            this.review.text = stringBuilder.ToString();
            customerName.text = review.CustomerName;
            stringBuilder.Clear();
        }

        private void SetFactors(ReviewLineInfo revInfo)
        {
            switch (revInfo.FactorType)
            {
                case ReviewFactorType.beauty:
                    beautyPoints.text = revInfo.Points.ToString();
                    break;

                case ReviewFactorType.notFound:
                    itemNotFoundPoints.text = revInfo.Points.ToString();
                    break;

                case ReviewFactorType.searchTime:
                    searchTimePoints.text = revInfo.Points.ToString();
                    break;
            }
        }
    }
}