using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Goat.Saving
{
    public class NarrativeSaveHandler : SaveHandler
    {
        public NarrativeManager narrativeManager;
        public BoolEvent OnNarrativeCompleted;

        public void Awake()
        {
            data = new NarrativeSaveData();
        }
    }

    public class NarrativeSaveData : DataContainer
    {
        public bool completed;

        public override void Load(SaveHandler handler)
        {
            NarrativeSaveHandler narrativeHandler = (NarrativeSaveHandler)handler;

            if (!completed)
                narrativeHandler.narrativeManager.enabled = true;
            else
                narrativeHandler.OnNarrativeCompleted.Raise();
        }

        public override void Save(SaveHandler handler)
        {
            NarrativeSaveHandler narrativeHandler = (NarrativeSaveHandler)handler;

            completed = narrativeHandler.narrativeManager.NarrativeFinished;
        }
    }
}
