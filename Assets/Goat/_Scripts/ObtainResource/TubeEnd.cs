using Sirenix.OdinInspector;
using UnityEngine;

namespace Goat.Farming
{
    public class TubeEnd : SerializedMonoBehaviour
    {
        [SerializeField] private TubeDirection tubeConnection;
        [SerializeField] private float radius = 0.1f;
        [SerializeField] private Vector3 pos;

        public Vector3 SpawnPos => tubeConnection.CorrectPosWithRotation(pos);
 
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(SpawnPos, radius);
        }
    }
}