using System.Collections.Generic;
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

    // Class for storing tile data
    public class Tile
    {
        private Vector3 centerPosition;
        private bool isUnlocked;
        private BuildingType buildingType;
        private FloorType floorType;
        private GameObject floorObject, buildingObject;
        private Grid grid;

        public Tile(Vector3 centerPosition, Grid grid) {
            this.centerPosition = centerPosition;
            this.grid = grid;
        }

        // Select this tile
        public void Select() {
            Debug.Log(GetTileInformation());
        }

        public void DeSelect() {

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
            floorObject = newObject != null ? GameObject.Instantiate(newObject, centerPosition, Quaternion.identity) : null;
            floorObject.transform.localScale = Vector3.one * grid.GetTileSize;
            this.floorType = floorType;
        }

        // Change the building type
        // Spawns a building object on this tile
        public void EditBuilding(BuildingType buildingType)
        {
            MonoBehaviour.Destroy(buildingObject);

            GameObject newObject = TileAssets.FindAsset(buildingType);
            buildingObject = newObject != null ? MonoBehaviour.Instantiate(newObject, centerPosition, Quaternion.identity) : null;
            buildingObject.transform.localScale = Vector3.one * grid.GetTileSize;
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