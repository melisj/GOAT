using Goat.Player;
using UnityEngine;

namespace Goat.Animation
{
    public class ThirdPersonAnimationController : AnimationController
    {
        [SerializeField] private CollisionDetection collisionDetection;

        protected override void Update()
        {
            base.Update();
            OnMove(collisionDetection.IsMoving ? 1 : 0);
        }
    }
}