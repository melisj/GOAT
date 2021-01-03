using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Saving
{
    public class ReputationSaveHandler : SaveHandler
    {
        public SatisfactionLevel satisfaction;

        private void Awake()
        {
            data = new ReputationSaveData();
        }
    }

    public class ReputationSaveData : DataContainer, ISaveable
    {
        public int satisfaction;

        public override void Load(SaveHandler handler)
        {
            ReputationSaveHandler reputationHandler = (ReputationSaveHandler)handler;

            reputationHandler.satisfaction.Satisfaction = satisfaction;
        }

        public override void Save(SaveHandler handler)
        {
            ReputationSaveHandler reputationHandler = (ReputationSaveHandler)handler;

            satisfaction = reputationHandler.satisfaction.Satisfaction;
        }
    }
}
