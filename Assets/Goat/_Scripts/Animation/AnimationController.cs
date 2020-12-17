using UnityEngine;

namespace Goat.Animation
{
    public class AnimationController : MonoBehaviour
    {
        protected const string MoveFloat = "Move";
        [SerializeField] private Animator animator;

        protected virtual void Update()
        {
        }

        protected virtual void OnMove(float moveAmount)
        {
            animator.SetFloat(MoveFloat, moveAmount);
        }

        protected virtual void OnInteract()
        {
        }
    }
}