using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using Goat.Events;

namespace Goat.AI.Parking
{
    public class NPCTimeBasedSpawner : EventListenerBool
    {
        [SerializeField] private TimeOfDay timeInfo;
        [SerializeField] private AnimationCurve spawnTimeCurve;
        [SerializeField] private float curveDefiation;
        [SerializeField] private ShipSpawner spawner;

        private bool day;
        private Coroutine spawnRoutine;

        public override void OnEventRaised(bool day)
        {
            this.day = day;
        }

        public void Update()
        {
            if (day)
            {
                if (spawnRoutine == null)
                    spawnRoutine = StartCoroutine(SpawnNpc());
            }
        }

        public IEnumerator SpawnNpc()
        {
            float timeBetweenDawnAndMorning = timeInfo.TimeOfSunset - timeInfo.TimeOfSunrise;
            float timeOfDay = timeInfo.TimeOfDayHours - timeInfo.TimeOfSunrise;

            int curveValue = (int)spawnTimeCurve.Evaluate(timeOfDay / (float)timeBetweenDawnAndMorning);
            float spawnTime = 5;
            if (curveValue != 0)
                spawnTime = (60 / curveValue) + Random.Range(-1, 1) * curveDefiation;
            float time = 0;

            while (time < spawnTime)
            {
                time += Time.deltaTime;
                yield return null;
            }

            if(curveValue != 0 && timeInfo.IsDay)
                spawner.SpawnShip(1);
            spawnRoutine = null;
        }
    }
}
