using System.Collections.Generic;
using UnityEngine;

namespace GOAT.Grid
{
    public enum BuildingType
    {
        Empty,
        Building
    }

    public enum FloorType
    {
        Empty,
        Wood,
        Glass,
        Steel
    }

    public class Tile
    {
        private Vector3 centerPosition;
        private bool isUnlocked;
        private BuildingType buildingType;
        private FloorType floorType;
        private GameObject floorObject, buildingObject;


        public Tile(Vector3 centerPosition) {
            this.centerPosition = centerPosition;
        }

        public void Select() {
            Debug.Log(GetTileInformation());
        }

        public void DeSelect() {

        }

        public TileInformation GetTileInformation()
        {
            // Initialize tile information
            return new TileInformation(centerPosition, floorType, buildingType);
        }

        public void EditFloor(FloorType floorType)
        {
            MonoBehaviour.Destroy(floorObject);
            GameObject newObject = TileAssets.FindAsset(floorType);
            floorObject = newObject != null ? GameObject.Instantiate(newObject, centerPosition, Quaternion.identity) : null;
            this.floorType = floorType;
            //isFloor = newObject != null;
        }

        public void EditBuilding(BuildingType buildingType)
        {
            MonoBehaviour.Destroy(buildingObject);

            GameObject newObject = TileAssets.FindAsset(buildingType);
            buildingObject = newObject != null ? MonoBehaviour.Instantiate(newObject, centerPosition, Quaternion.identity) : null;
            this.buildingType = buildingType;
        }
    }

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