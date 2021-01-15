using Goat.Grid;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Goat.Saving
{
    public class GridSaveHandler : SaveHandler
    {
        public Grid.Grid grid;
        public GridObjectsList objectList;
        public VoidEvent onGridChange;

        public void Awake()
        {
            data = new GridSaveData();
        }

        public override void Load(DataContainer data)
        {
            grid.Reset();
            base.Load(data);
        }

        public void StartSpawingTiles(List<TileInfo> tiles)
        {
            StartCoroutine(CoroutineSpawnTile(tiles));
        }

        public IEnumerator CoroutineSpawnTile(List<TileInfo> tiles)
        {
            // Load in all the data on the tiles
            for (int x = 0; x < grid.GetGridSize.x; x++)
            {
                for (int y = 0; y < grid.GetGridSize.y; y++)
                {
                    if (tiles[grid.GetGridSize.y * x + y].empty)
                        continue;

                    grid.tiles[x, y].LoadInData(tiles[grid.GetGridSize.y * x + y], ref objectList);
                    yield return null;
                }
            }
        }
    }

    [Serializable]
    public class GridSaveData : DataContainer, ISaveable
    {
        public List<TileInfo> tileData = new List<TileInfo>();

        public override void Load(SaveHandler handler)
        {
            GridSaveHandler gridHandler = (GridSaveHandler)handler;

            // Check dimensions before loading
            if (gridHandler.grid.GetGridSize.x * gridHandler.grid.GetGridSize.y != tileData.Count)
            {
                Debug.LogWarning("Could not load grid, grid size is not the same!");
                return;
            }

            gridHandler.StartSpawingTiles(tileData);
        }

        public override void Save(SaveHandler handler)
        {
            GridSaveHandler gridHandler = (GridSaveHandler)handler;
            tileData.Clear();

            for (int x = 0; x < gridHandler.grid.tiles.GetLength(0); x++)
            {
                for (int y = 0; y < gridHandler.grid.tiles.GetLength(1); y++)
                {
                    gridHandler.grid.tiles[x, y].SaveAllData();
                    tileData.Add(gridHandler.grid.tiles[x, y].SaveData);
                }
            }
        }
    }
}