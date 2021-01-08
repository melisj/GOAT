using UnityEngine;
using UnityEngine.AI;
using Goat.Events;

namespace Goat.AI
{
    public class UpdateNavOnInputModeChange : EventListenerInputMode
    {
        [SerializeField] private NavMeshSurface surfaceAI;
        [SerializeField] private NavMeshSurface surfacePlayer;
        [SerializeField] private NavMeshSurface surfaceWorker;

        public override void OnEventRaised(InputMode value)
        {
            if (value != InputMode.Edit)
            {
                if(surfaceAI.navMeshData)
                    surfaceAI.UpdateNavMesh(surfaceAI.navMeshData);
                if (surfacePlayer.navMeshData)
                    surfacePlayer.UpdateNavMesh(surfacePlayer.navMeshData);
                if (surfaceWorker.navMeshData)
                    surfaceWorker.UpdateNavMesh(surfaceWorker.navMeshData);
            }
        }
    }
}