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

        public void Awake()
        {
            data = new GridSaveData();
        }

        public override IEnumerator Load(DataHandler handler, DataContainer data)
        {
            grid.Reset();
            StartCoroutine(base.Load(handler, data));
            yield break;
        }
    }

    [Serializable]
    public class GridSaveData : DataContainer, ISaveable
    {
        public List<TileInfo> tileData = new List<TileInfo>();

        public override IEnumerator Load(SaveHandler handler)
        {
            GridSaveHandler gridHandler = (GridSaveHandler)handler;

            // Check dimensions before loading
            if (gridHandler.grid.GetGridSize.x * gridHandler.grid.GetGridSize.y != tileData.Count)
            {
                Debug.LogWarning("Could not load grid, grid size is not the same!");
                DoneLoading(handler, DataHandler.ContainerExitCode.Failure);
            }

            // Load in all the data on the tiles
            for (int x = 0; x < gridHandler.grid.GetGridSize.x; x++)
            {
                for (int y = 0; y < gridHandler.grid.GetGridSize.y; y++)
                {
                    if (tileData[gridHandler.grid.GetGridSize.y * x + y].empty)
                        continue;

                    gridHandler.grid.tiles[x, y].LoadInData(tileData[gridHandler.grid.GetGridSize.y * x + y], ref gridHandler.objectList);
                    yield return null;
                }
            }
            DoneLoading(handler, DataHandler.ContainerExitCode.Success);
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