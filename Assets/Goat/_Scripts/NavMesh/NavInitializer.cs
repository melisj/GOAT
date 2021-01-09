using Goat.Saving;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

namespace Goat.AI
{
    public class NavInitializer : MonoBehaviour
    {
        [SerializeField] private NavMeshSurface[] surfaces;

        [Button]
        private void FillSurfacesArray()
        {
            surfaces = GetComponentsInChildren<NavMeshSurface>();
        }

        private void Start()
        {
            DataHandler.LevelLoaded += GridDataHandler_LevelLoaded;

            for (int i = 0; i < surfaces.Length; i++)
            {
                surfaces[i].BuildNavMesh();
            }
        }

        private void GridDataHandler_LevelLoaded()
        {
            RebakeMesh();
        }

        private void RebakeMesh()
        {
            for (int i = 0; i < surfaces.Length; i++)
            {
                surfaces[i]?.UpdateNavMesh(surfaces[i]?.navMeshData);
            }
        }
    }
}