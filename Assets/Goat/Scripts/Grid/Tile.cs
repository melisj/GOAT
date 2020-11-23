﻿using System.Collections.Generic;
using UnityEngine;

namespace GOAT.Grid
{
    public enum BuildingType
    {
        Empty,
        Building,
        lowtable,
        table
    }

    public enum FloorType
    {
        Empty,
        Wood,
        Glass,
        Steel
    }

    public enum WallPosition
    {
        Left = 270,
        Right = 90,
        Top = 0,
        Bottom = 180
    }

    public enum WallType
    {
        Empty,
        WoodSolid,
        WoodDoor,
        WoodWindow
    }

    // Class for storing tile data
    public class Tile
    {
        private Vector3 centerPosition;
        private bool isUnlocked;
        private BuildingType buildingType;
        private FloorType floorType;
        private GameObject floorObject, buildingObject;
        private Grid grid;
        private Dictionary<WallPosition, WallType> wallPositions = new Dictionary<WallPosition, WallType>();
        private Dictionary<WallPosition, GameObject> wallObjects = new Dictionary<WallPosition, GameObject>();


        public Tile(Vector3 centerPosition, Grid grid)
        {
            this.centerPosition = centerPosition;
            this.grid = grid;
            InitializeWalls();
        }

        private void InitializeWalls()
        {
            wallPositions.Add(WallPosition.Left, WallType.Empty);
            wallObjects.Add(WallPosition.Left, null);
            wallPositions.Add(WallPosition.Right, WallType.Empty);
            wallObjects.Add(WallPosition.Right, null);
            wallPositions.Add(WallPosition.Top, WallType.Empty);
            wallObjects.Add(WallPosition.Top, null);
            wallPositions.Add(WallPosition.Bottom, WallType.Empty);
            wallObjects.Add(WallPosition.Bottom, null);
        }

        public void EditWall(WallPosition position, WallType type)
        {
            if(wallPositions[position] != type)
            {
                if (wallObjects[position]) MonoBehaviour.Destroy(wallObjects[position]);
                // Find gameobject
                //GameObject newObject =TileAssets.FindAsset(WallType);
                // Instantiate gameobject
                //wallObjects[position] = GameObject.Instantiate(newObject, centerPosition, Quaterion.AngleAxis((float)(int)position, Vector3.up));
                //wallObjects[position].transform.localScale = Vector3.one * grid.GetTileSize;
            }
        }

        // Select this tile
        public void Select()
        {
            Debug.Log(GetTileInformation());
        }

        public void DeSelect()
        {

        }

        public void ShowFloor(bool show)
        {
            if (floorObject != null)
                floorObject.SetActive(show);
        }
        public void ShowBuilding(bool show)
        {
            if (buildingObject != null)
                buildingObject.SetActive(show);
        }

        // Returns a tile info struct
        public TileInformation GetTileInformation()
        {
            // Initialize tile information
            return new TileInformation(centerPosition, floorType, buildingType);
        }

        // Change the tile type 
        // Spawns a floor object on this tile
        public void EditFloor(FloorType floorType)
        {
            MonoBehaviour.Destroy(floorObject);
            GameObject newObject = TileAssets.FindAsset(floorType);
            if (newObject)
            {
                floorObject = GameObject.Instantiate(newObject, centerPosition, Quaternion.identity);
                floorObject.transform.localScale = Vector3.one * grid.GetTileSize;
            }
            this.floorType = floorType;
        }

        // Change the building type
        // Spawns a building object on this tile
        public void EditBuilding(BuildingType buildingType, Quaternion buildingRotation)
        {
            MonoBehaviour.Destroy(buildingObject);

            GameObject newObject = TileAssets.FindAsset(buildingType);
            if (newObject)
            {
                buildingObject = MonoBehaviour.Instantiate(newObject, centerPosition, buildingRotation);
                buildingObject.transform.localScale = Vector3.one * grid.GetTileSize;
            }
            this.buildingType = buildingType;
        }




    }

    // Structure for storing data
    public struct TileInformation
    {
        public readonly Vector3 TilePosition;
        public readonly FloorType floorType;
        public readonly BuildingType buildingType;

        public TileInformation(Vector3 TilePosition, FloorType floorType, BuildingType buildingType)
        {
            this.TilePosition = TilePosition;
            this.floorType = floorType;
            this.buildingType = buildingType;
        }
    }
}