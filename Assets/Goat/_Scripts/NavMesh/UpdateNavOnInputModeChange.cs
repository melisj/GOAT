using UnityEngine;
using UnityEngine.AI;
using Goat.Events;
using Sirenix.OdinInspector;

namespace Goat.AI
{
    public class UpdateNavOnInputModeChange : EventListenerInputMode
    {
        [SerializeField] private NavMeshSurface[] surfaces;

        [Button]
        private void FillSurfaceArray()
        {
            surfaces = GetComponentsInChildren<NavMeshSurface>();
        }

        public override void OnEventRaised(InputMode value)
        {
            if (value != InputMode.Edit)
            {
                for (int i = 0; i < surfaces.Length; i++)
                {
                    surfaces[i]?.UpdateNavMesh(surfaces[i]?.navMeshData);
                }
            }
        }
    }
}