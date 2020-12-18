using UnityEngine;
using UnityEngine.AI;
using Goat.Events;

namespace Goat.AI
{
    public class UpdateNavOnInput : EventListenerKeyCodeModeEvent
    {
        [SerializeField] private NavMeshSurface surfaceAI;
        [SerializeField] private NavMeshSurface surfacePlayer;

        public override void OnEventRaised(KeyCodeMode value)
        {
            KeyCode code = KeyCode.None;
            KeyMode mode = KeyMode.None;

            value.Deconstruct(out code, out mode);

            if (code == KeyCode.N && mode == KeyMode.Down)
            {
                surfaceAI.UpdateNavMesh(surfaceAI.navMeshData);
                surfacePlayer.UpdateNavMesh(surfacePlayer.navMeshData);
            }
        }
    }
}