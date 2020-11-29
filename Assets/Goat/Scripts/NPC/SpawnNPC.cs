using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using Goat.Pooling;
using Goat.Manager;

public class SpawnNPC : MonoBehaviour
{
    [SerializeField] private GameObject npcPrefab;
    private Sequence sequence;

    private void Awake()
    {
        sequence = DOTween.Sequence();
    }

    [Button("Spawn NPC")]
    private void Spawn()
    {
        //Instantiate(npcPrefab, transform.position, Quaternion.identity);
        if (NpcManager.Instance.AvailableResources.Keys.Count <= 0) return;
        GameObject npc = PoolManager.Instance.GetFromPool(npcPrefab, transform.position, Quaternion.identity);
        NPCScript npcScript = npc.GetComponent<NPCScript>();
        npcScript.Setup();
    }

    public void KillSequence()
    {
        sequence.Kill(true);
    }

    public void SpawnRepeat(int amount, int interval)
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