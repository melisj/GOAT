using Goat.Events;
using UnityEngine;

namespace Goat.Grid
{
    public class CheckForDestroyMode : EventListenerInputMode
    {
        [SerializeField] private Grid grid;

        public override void OnEventRaised(InputMode value)
        {
            grid.DestroyModeChange(value);
        }
    }
}