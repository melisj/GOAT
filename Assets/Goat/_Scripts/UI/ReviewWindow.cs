using System.Collections.Generic;
using UnityEngine;
using Goat.Grid.UI;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using Sirenix.OdinInspector;
using Goat.Pooling;
using TMPro;

namespace Goat.UI
{
    public class ReviewWindow : BasicGridUIElement, IAtomListener<Review>, IAtomListener<bool>
    {
        [SerializeField] private GameObject reviewCellPrefab;
        [SerializeField] private Transform grid;
        [SerializeField] private ReviewEvent revEvent;
        [SerializeField] private bool clearReviewsAtDay;
        [SerializeField] private IntEvent onReviewAdded;
        [SerializeField, ShowIf("clearReviewsAtDay")] private BoolEvent onDay;
        private List<GameObject> revCellsCreated = new List<GameObject>();

        private void OnEnable()
        {
            if (revEvent == null) return;
            revEvent.RegisterListener(GetComponent<IAtomListener<Review>>());
            onDay.RegisterListener(GetComponent<IAtomListener<bool>>());
        }

        private void OnDisable()
        {
            if (revEvent == null) return;
            revEvent.UnregisterListener(GetComponent<IAtomListener<Review>>());
            onDay.UnregisterListener(GetComponent<IAtomListener<bool>>());
        }

        public void OnEventRaised(bool isDay)
        {
            if (clearReviewsAtDay && isDay)
            {
                ClearReviewCells();
            }
        }

        private void ClearReviewCells()
        {
            for (int i = 0; i < revCellsCreated.Count; i++)
            {
                GameObject cell = revCellsCreated[i];
                PoolManager.Instance.ReturnToPool(cell);
            }
            revCellsCreated.Clear();
            onReviewAdded.Raise(revCellsCreated.Count);
        }

        public void OnEventRaised(Review rev)
        {
            CreateReviewCell(rev);
        }

        private void CreateReviewCell(Review rev)
        {
            GameObject cell = PoolManager.Instance.GetFromPool(reviewCellPrefab, grid);
            revCellsCreated.Add(cell);
            cell.transform.SetAsFirstSibling();
            ReviewCell cellScript = cell.GetComponent<ReviewCell>();
            cellScript.Setup(rev);
            onReviewAdded.Raise(revCellsCreated.Count);
        }
    }
}