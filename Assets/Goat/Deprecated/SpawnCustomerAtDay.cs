using UnityEngine;
using Goat.Events;
using DG.Tweening;
using Goat.Manager;
using Goat.Pooling;

namespace Goat
{
    public class SpawnCustomerAtDay : EventListenerBool
    {
        [SerializeField] private GameObject npcPrefab;
        private Sequence sequence;

        public override void OnEventRaised(bool value)
        {
            if (value)
            {
                SpawnRepeat(1, 5);
            }
            else
            {
                KillSequence();
            }
        }

        private void Awake()
        {
            sequence = DOTween.Sequence();
        }

        private void Spawn()
        {
            //Instantiate(npcPrefab, transform.position, Quaternion.identity);
            if (NpcManager.Instance.AvailableResources.Keys.Count <= 0) return;
            GameObject npc = PoolManager.Instance.GetFromPool(npcPrefab, transform.position, Quaternion.identity);
            NPCScript npcScript = npc.GetComponent<NPCScript>();
            npcScript.Setup(transform);
        }

        private void KillSequence()
        {
            sequence.Kill(true);
        }

        private void SpawnRepeat(int amount, int interval)
        {
            sequence.Append(DOTween.To(() => interval, x => interval = x, 0, interval).SetLoops(60, LoopType.Restart).OnStepComplete(() =>
            {
                for (int i = 0; i < amount; i++)
                {
                    Spawn();
                }
            }));
        }
    }
}