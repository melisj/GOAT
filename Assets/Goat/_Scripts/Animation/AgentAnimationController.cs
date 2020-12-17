using UnityEngine;
using UnityEngine.AI;

namespace Goat.Animation
{
    public class AgentAnimationController : AnimationController
    {
        [SerializeField] private NavMeshAgent agent;

        protected override void Update()
        {
            base.Update();
            OnMove(agent.velocity.sqrMagnitude);
        }
    }
}