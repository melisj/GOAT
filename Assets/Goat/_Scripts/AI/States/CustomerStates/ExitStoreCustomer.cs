using Goat.AI.Satisfaction;
using UnityEngine;
using UnityEngine.AI;

namespace Goat.AI.States
{
    public class ExitStoreCustomer : ExitStore
    {
        private CustomerReview review;

        public ExitStoreCustomer(NPC npc, NavMeshAgent navMeshAgent, Animator animator, CustomerReview review) : base(npc, navMeshAgent, animator)
        {
            this.npc = npc;
            this.navMeshAgent = navMeshAgent;
            this.animator = animator;
            this.review = review;
        }

        public override void OnEnter()
        {
            base.OnEnter();
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