using Goat.AI.Satisfaction;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.AI;

namespace Goat.AI.States
{
    public class ExitStoreCustomer : ExitStore
    {
        private CustomerReview review;
        private IntVariable customerCapacity;
        private AudioCue checkout, angry;

        public ExitStoreCustomer(NPC npc, NavMeshAgent navMeshAgent, CustomerReview review, IntVariable customerCapacity, AudioCue checkout, AudioCue angry) : base(npc, navMeshAgent)
        {
            this.npc = npc;
            this.navMeshAgent = navMeshAgent;
            this.review = review;
            this.customerCapacity = customerCapacity;
            this.checkout = checkout;
            this.angry = angry;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            customerCapacity.Subtract(1);
            if (npc is Customer customer)
            {
                if (customer.OutofTime)
                    angry.PlayAudioCue();
                else
                    checkout.PlayAudioCue();
            }
        }

        public override void OnExit()
        {
            review.WriteReview();
            base.OnExit();
        }

        public override void Tick()
        {
            base.Tick();
        }
    }
}