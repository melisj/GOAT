using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class ReviewButton : CellWithInventoryAmount, IAtomListener<int>
{
    [SerializeField] private IntEvent onReviewAdded;

    private void OnEnable()
    {
        onReviewAdded.RegisterSafe(this);
    }

    private void OnDisable()
    {
        onReviewAdded.UnregisterSafe(this);
    }

    public void OnEventRaised(int amountReviews)
    {
        Setup(amountReviews);
    }
}