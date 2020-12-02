using UnityEngine;
using Goat.Events;

namespace Goat
{
    public class SpawnCustomerAtDay : EventListenerBool
    {
        [SerializeField] private SpawnNPC npcSpawner;

        public override void OnEventRaised(bool value)
        {
            if (value)
            {
                npcSpawner.SpawnRepeat(1, 5);
            }
            else
            {
                npcSpawner.KillSequence();
            }
        }
    }
}