using UnityEngine;
using UnityEngine.AI;
using Goat.Events;
using Sirenix.OdinInspector;

namespace Goat.AI
{
    public class UpdateNavOnInput : MonoBehaviour
    {
        [SerializeField] private NavMeshSurface[] surfaces;

        [Button]
        private void FillSurfacesArray()
        {
            surfaces = GetComponentsInChildren<NavMeshSurface>();
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                for (int i = 0; i < surfaces.Length; i++)
                {
                    surfaces[i]?.UpdateNavMesh(surfaces[i]?.navMeshData);
                }
            }
        }
    }

#endif
}