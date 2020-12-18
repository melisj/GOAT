using UnityEngine;
using UnityEngine.AI;
using Goat.Events;

namespace Goat.AI
{
    public class UpdateNavOnInputModeChange : EventListenerInputMode
    {
        [SerializeField] private NavMeshSurface surfaceAI;
        [SerializeField] private NavMeshSurface surfacePlayer;

        public override void OnEventRaised(InputMode value)
        {
            if (value != InputMode.Edit)
            {
                surfaceAI?.UpdateNavMesh(surfaceAI?.navMeshData);
                surfacePlayer?.UpdateNavMesh(surfacePlayer?.navMeshData);
            }
        }
    }
}