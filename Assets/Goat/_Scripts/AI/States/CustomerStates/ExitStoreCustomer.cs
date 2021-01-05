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

        public ExitStoreCustomer(NPC npc, NavMeshAgent navMeshAgent, Animator animator, CustomerReview review, IntVariable customerCapacity) : base(npc, navMeshAgent, animator)
        {
            this.npc = npc;
            this.navMeshAgent = navMeshAgent;
            this.animator = animator;
            this.review = review;
            this.customerCapacity = customerCapacity;
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnExit()
        {
            review.WriteReview();
            customerCapacity.Subtract(1);
            base.OnExit();
        }

        public override void Tick()
        {
            base.Tick();
        }
    }
}