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
            for (int i = 0; i < surfaces.Length; i++)
            {
                surfaces[i].BuildNavMesh();
            }
        }

        private void OnEnable()
        {
            DataHandler.LevelLoaded += GridDataHandler_LevelLoaded;
        }

        private void OnDisable()
        {
            DataHandler.LevelLoaded -= GridDataHandler_LevelLoaded;
        }

        private void GridDataHandler_LevelLoaded()
        {
            RebakeMesh();
        }

        public void RebakeMesh()
        {
            for (int i = 0; i < surfaces.Length; i++)
            {
                if (surfaces[i].navMeshData)
                    surfaces[i].UpdateNavMesh(surfaces[i].navMeshData);
            }
        }
    }
}