using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SpawnNPC : MonoBehaviour
{
    [SerializeField] private GameObject npcPrefab;

    [Button("Spawn NPC")]
    private void Spawn() {
        Instantiate(npcPrefab, transform.position, Quaternion.identity);
    }
}
