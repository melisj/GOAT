using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using Goat.Events;

namespace Goat.AI.Parking
{
    public class NPCTimeBasedSpawner : EventListenerBool
    {
        [SerializeField] private Vector2 spawnTimeRange;
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
            float spawnTime = Random.Range(spawnTimeRange.x, spawnTimeRange.y);
            float time = 0;

            while (time < spawnTime)
            {
                time += Time.deltaTime;
                yield return null;
            }

            spawner.SpawnShip(1);
            spawnRoutine = null;
        }
    }
}
