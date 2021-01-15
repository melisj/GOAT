using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.AI;

namespace Goat.AI.States
{
    public class EnterStoreCustomer : EnterStore
    {
        private IntVariable customerCapacity;

        public EnterStoreCustomer(NPC npc, NavMeshAgent navMeshAgent, UnloadLocations entrances, IntVariable customerCapacity) : base(npc, navMeshAgent, entrances)
        {
            this.npc = npc;
            this.navMeshAgent = navMeshAgent;
            this.entrances = entrances;
            this.customerCapacity = customerCapacity;
        }

        public override void OnExit()
        {
            customerCapacity.Add(1);
            base.OnExit();
        }
    }
}