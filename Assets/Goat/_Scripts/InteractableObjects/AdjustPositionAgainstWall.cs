using UnityEngine;

namespace Goat.Grid.Interactions
{
    public class AdjustPositionAgainstWall : MonoBehaviour
    {
        [SerializeField] private float offset = -0.1f;
        private Vector3 spawnPosition;

        public void Setup()
        {
            spawnPosition = transform.position;
        }

        public void AdjustPosition()
        {
            transform.position += transform.forward * offset;
        }

        public void ResetPosition()
        {
            transform.position = spawnPosition;
        }
    }
}