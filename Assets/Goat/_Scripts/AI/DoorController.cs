using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private bool ring = false;
    [SerializeField] private DoorRingSound doorRing;
    [SerializeField] private DoorOpenCloseSound doorOpenClose;

    [SerializeField] private int agentsInCollider = 0;

    private void OnTriggerEnter(Collider other)
    {
        if(agentsInCollider == 0)
        {
            doorRing.PlayAudio();
            doorOpenClose.PlayAudio();
        }

        agentsInCollider++;
        //doorAnimator.SetInteger("AgentsInCollider", agentsInCollider);
    }
    private void OnTriggerExit(Collider other)
    {
        agentsInCollider--;
        //doorAnimator.SetInteger("AgentsInCollider", agentsInCollider);

        if (agentsInCollider == 0)
        {
            doorRing.PlayAudio();
            doorOpenClose.PlayAudio();
        }
    }
}
