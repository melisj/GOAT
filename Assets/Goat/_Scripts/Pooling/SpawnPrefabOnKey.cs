using UnityEngine;
using System.Collections;

namespace Goat.Pooling
{
    public class SpawnPrefabOnKey : MonoBehaviour
    {
        [SerializeField] private GameObject m_Prefab;
        [SerializeField] private UnloadLocations entrances;
        [SerializeField] private KeyCode m_KeyCode;
        [SerializeField] private int amountSpawned;

        private void Update()
        {
            if (Input.GetKeyDown(m_KeyCode) && m_Prefab != null)
            {
                //Instantiate(m_Prefab, transform.position, transform.rotation);
                if (entrances.Locations.Count > 0)
                {
                    Vector3 pos = entrances.Locations[Random.Range(0, entrances.Locations.Count)];
                    PoolManager.Instance.GetFromPool(m_Prefab, pos, Quaternion.identity);
                    amountSpawned++;
                }
            }
        }
    }
}