using Goat.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Saving
{
    public class MoneySaveHandler : SaveHandler
    {
        public Money money;

        private void Awake()
        {
            data = new MoneySaveData();
        }
    }

    public class MoneySaveData : DataContainer, ISaveable
    {
        public float moneyAmount;

        public override IEnumerator Load(SaveHandler handler)
        {
            MoneySaveHandler moneyHandler = (MoneySaveHandler)handler;

            moneyHandler.money.Amount = moneyAmount;

            DoneLoading(handler, DataHandler.ContainerExitCode.Success);
            yield break;
        }

        public override void Save(SaveHandler handler)
        {
            MoneySaveHandler moneyHandler = (MoneySaveHandler)handler;

            moneyAmount = moneyHandler.money.Amount;
        }
    }
}