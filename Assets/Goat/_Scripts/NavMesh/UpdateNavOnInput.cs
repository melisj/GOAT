using UnityEngine;
using UnityEngine.AI;
using Goat.Events;
using Sirenix.OdinInspector;

namespace Goat.AI
{
    public class UpdateNavOnInput : MonoBehaviour
    {
        [SerializeField] private NavInitializer navInitializer;

#if UNITY_EDITOR || DEVELOPMENT_BUILD

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                navInitializer.RebakeMesh();
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                navInitializer.BakeNavMesh();
            }
        }

#endif
    }
}