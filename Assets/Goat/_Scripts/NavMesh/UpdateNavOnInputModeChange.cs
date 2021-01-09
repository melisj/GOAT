using UnityEngine;
using UnityEngine.AI;
using Goat.Events;
using Sirenix.OdinInspector;

namespace Goat.AI
{
    public class UpdateNavOnInputModeChange : EventListenerInputMode
    {
        [SerializeField] private NavInitializer navInitializer;

        public override void OnEventRaised(InputMode value)
        {
            if (value != InputMode.Edit)
            {
                navInitializer.RebakeMesh();
            }
        }
    }
}