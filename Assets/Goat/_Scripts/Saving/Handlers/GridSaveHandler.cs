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

            // Load in all the data on the tiles
            for (int x = 0; x < gridHandler.grid.GetGridSize.x; x++)
            {
                for (int y = 0; y < gridHandler.grid.GetGridSize.y; y++)
                {
                    gridHandler.grid.tiles[x, y].LoadInData(tileData[gridHandler.grid.GetGridSize.y * x + y], ref gridHandler.objectList);
                }
            }
        }

        public override void Save(SaveHandler handler)
        {
            GridSaveHandler gridHandler = (GridSaveHandler)handler;
            tileData.Clear();

            for (int x = 0; x < gridHandler.grid.tiles.GetLength(0); x++)
            {
                for (int y = 0; y < gridHandler.grid.tiles.GetLength(1); y++)
                {
                    gridHandler.grid.tiles[x, y].SaveStorageData();
                    tileData.Add(gridHandler.grid.tiles[x, y].SaveData);
                }
            }
        }
    }
}