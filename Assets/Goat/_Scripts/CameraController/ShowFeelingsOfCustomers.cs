using Goat.AI.Feelings;
using UnityEngine;

public class ShowFeelingsOfCustomers : CollisionDetection
{
    [SerializeField] private CustomerFeelings feelings;

    public override void OnEnter()
    {
        if (!latestCollider) return;
        if (previousCollider != latestCollider)
        {
            feelings.TransitionUI(true);
        }
    }

    public override void OnExit()
    {
        if (!previousCollider) return;

        feelings.TransitionUI(false);
    }
}