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
        private bool isFloor;
        private bool isUnlocked;
        private BuildingType buildingType;
        private FloorType floorType;
        private GameObject floorObject, buildingObject;

        public bool IsFloor { get => isFloor; }

        public Tile(Vector3 centerPosition) {
            this.centerPosition = centerPosition;
        }

        public void Select() {
            GetTileInformation();
        }

        public void DeSelect() {

        }

        public TileInformation GetTileInformation()
        {
            // Initialize tile information
            return new TileInformation(centerPosition, buildingType);
        }

        public void EditFloor(FloorType floorType)
        {
            MonoBehaviour.Destroy(floorObject);
            //floorObject = GameObject.Instantiate(newObject, centerPosition, Quaternion.identity);

            //isFloor = newObject != null;
        }

        public void EditBuilding(BuildingType buildingType)
        {
            MonoBehaviour.Destroy(buildingObject);

            //buildingObject = MonoBehaviour.Instantiate(newObject, centerPosition, Quaternion.identity);
            this.buildingType = buildingType;
        }
    }

    public struct TileInformation
    {
        public readonly Vector3 TilePosition;
        public readonly BuildingType buildingType;

        public TileInformation(Vector3 TilePosition, BuildingType buildingType)
        {
            this.TilePosition = TilePosition;
            this.buildingType = buildingType;
        }
    }
}