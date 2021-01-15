using Grid = Goat.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Grid;
using Goat.Pooling;

public class MeteorMovement : MonoBehaviour, IPoolObject
{
    [SerializeField] GameObject explosion;
    [SerializeField] private int explosionRadius = 5;
    [SerializeField] private Placeable resourceTile;

    public int PoolKey { get; set; }
    public ObjectInstance ObjInstance { get; set; }

    private Grid.Grid grid;
    private Tile tileHit;

    void Update()
    {
        this.transform.position -= transform.up * 10 * Time.deltaTime;
        Explode();
    }

    /*void Explode()
    {
        if (transform.position.y < -3)
        {
            Instantiate(explosion, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);

            SpawnResourceTiles();
        }
        //doe particle

    }*/

    void Explode()
    {
        if (transform.position.y < 0)
        {
            PoolManager.Instance.GetFromPool(explosion, transform.position, Quaternion.identity);
            PoolManager.Instance.ReturnToPool(gameObject);
            SpawnResourceTiles();
        }
    }

    public void InitMeteor(Grid.Grid grid, Placeable resourceTile)
    {
        this.grid = grid;
        this.resourceTile = resourceTile;
        GetSpawn();
    }

    private void GetSpawn()
    {
        tileHit = grid.GetRandomEmptyTile();
        if(tileHit != null)
            transform.position = tileHit.Position + transform.up * 20;
    }

    private void SpawnResourceTiles()
    {
        List<Tile> tiles = grid.GetTilesInRange(tileHit, explosionRadius, false);
        foreach(Tile tile in tiles)
        {
            if (tile.HasNoObjects)
                tile.EditAny(resourceTile, Random.Range(0, 4) * 90, false);
        }
    }

    public void OnGetObject(ObjectInstance objectInstance, int poolKey)
    {
        PoolKey = poolKey;
        ObjInstance = objectInstance;
    }

    public void OnReturnObject()
    {
        gameObject.SetActive(false);
    }
}
