using Goat.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace Goat.AI
{
    public class NavInitializer : MonoBehaviour
    {
        [SerializeField] private NavMeshSurface surfaceAI;
        [SerializeField] private NavMeshSurface surfacePlayer;
        [SerializeField] private NavMeshSurface surfaceWorker;

        private void Start()
        {
            surfaceAI.BuildNavMesh();
            surfacePlayer.BuildNavMesh();
            surfaceWorker.BuildNavMesh();
        }

        private void OnEnable()
        {
            DataHandler.LevelLoaded += GridDataHandler_LevelLoaded;
        }

        private void OnDisable ()
        {
            DataHandler.LevelLoaded -= GridDataHandler_LevelLoaded;
        }

        private void GridDataHandler_LevelLoaded()
        {
            RebakeMesh();
        }

        private void RebakeMesh()
        {
            surfaceAI.UpdateNavMesh(surfaceAI.navMeshData);
            surfacePlayer.UpdateNavMesh(surfacePlayer.navMeshData);
            surfaceWorker.UpdateNavMesh(surfaceWorker.navMeshData);
        }
    }
}