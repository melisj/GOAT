using Goat.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class MeteorSpawner : MonoBehaviour, IAtomListener<bool>
{
    [SerializeField] private GameObject meteoritePrefab;
    [SerializeField] private Goat.Grid.Grid grid;
    [SerializeField] private BoolEvent onCycleChange;

    [Header("Settings")]
    [SerializeField] private int spawnPerNight = 2;
    [SerializeField] private Vector2 randomSpawnTime;

    private int spawnedThisNight = 0;
    ResourceTileData[] resourceTiles;

    private bool canSpawn => spawnRoutine == null && spawnedThisNight < spawnPerNight && isNight;
    private bool isNight;
    private Coroutine spawnRoutine;

    private void OnEnable()
    {
        resourceTiles = Resources.LoadAll<ResourceTileData>("ResourceTiles");
        onCycleChange.RegisterSafe(this);
    }

    private void OnDisable()
    {
        onCycleChange.UnregisterSafe(this);
    }

    public void OnEventRaised(bool isDay)
    {
        isNight = !isDay;
        spawnedThisNight = 0;
    }

    private void Update()
    {
        if(canSpawn)
            spawnRoutine = StartCoroutine(SpawnMeteor());
    }

    private IEnumerator SpawnMeteor()
    {
        float spawnTime = Random.Range(randomSpawnTime.x, randomSpawnTime.y), timer = 0;
        while(timer < spawnTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        InitMeteor();

        spawnRoutine = null;
    }

    private void InitMeteor()
    {
        int randomResourceTile = Random.Range(0, resourceTiles.Length);

        GameObject meteorObj = PoolManager.Instance.GetFromPool(meteoritePrefab);
        MeteorMovement meteor = meteorObj.GetComponent<MeteorMovement>();

        meteor.InitMeteor(grid, resourceTiles[randomResourceTile]);
        spawnedThisNight++;
    }
}
