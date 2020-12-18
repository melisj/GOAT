using Goat.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Goat.AI
{
    public class NavInitializer : MonoBehaviour
    {
        [SerializeField] private NavMeshSurface surfaceAI;
        [SerializeField] private NavMeshSurface surfacePlayer;

        private void Awake()
        {
            GridDataHandler.LevelLoaded += GridDataHandler_LevelLoaded;
            surfaceAI.BuildNavMesh();
            surfacePlayer.BuildNavMesh();
        }

        private void GridDataHandler_LevelLoaded()
        {
            RebakeMesh();
        }

        private void RebakeMesh()
        {
            surfaceAI.UpdateNavMesh(surfaceAI.navMeshData);
            surfacePlayer.UpdateNavMesh(surfacePlayer.navMeshData);
        }
    }
}