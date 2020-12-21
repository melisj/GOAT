using Goat.AI.Feelings;
using UnityEngine;

public class ShowFeelingsOfCustomers : CollisionDetection
{
    [SerializeField] private CustomerFeelings feelings;

    public override void OnEnter()
    {
        if (latestCollider)
        {
            feelings.TransitionUI(true);
        }

        //for (int i = 0; i < allColliders.Length; i++)
        //{
        //    if (allColliders[i] == null) continue;
        //    CustomerFeelings feelings = allColliders[i].GetComponent<CustomerFeelings>();
        //    if (feelings)
        //    {
        //        feelings.TransitionUI(true);
        //    }
        //}
    }

    public override void OnExit()
    {
        feelings.TransitionUI(false);

        //if (allPreviousColliders == null) return;
        //for (int i = 0; i < allPreviousColliders.Length; i++)
        //{
        //    if (allPreviousColliders[i] == null) continue;
        //    if (allPreviousColliders[i] == allColliders[i]) continue;
        //    CustomerFeelings feelings = allPreviousColliders[i].GetComponent<CustomerFeelings>();
        //    if (feelings)
        //    {
        //        feelings.TransitionUI(false);
        //    }
        //}
    }
}