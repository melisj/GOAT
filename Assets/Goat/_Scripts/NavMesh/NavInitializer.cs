using Goat.Saving;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using UnityAtoms;
using UnityAtoms.BaseAtoms;

namespace Goat.AI
{
    public class NavInitializer : MonoBehaviour, IAtomListener<Void>
    {
        [SerializeField] private NavMeshSurface[] surfaces;
        [SerializeField] private VoidEvent onLevelLoaded;

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

        public void BakeNavMesh()
        {
            for (int i = 0; i < surfaces.Length; i++)
            {
                surfaces[i].RemoveData();
                surfaces[i].BuildNavMesh();
            }
        }

        private void OnEnable()
        {
            onLevelLoaded.RegisterSafe(this);
        }

        private void OnDisable()
        {
            onLevelLoaded.UnregisterSafe(this);
        }

        public void OnEventRaised(Void item)
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