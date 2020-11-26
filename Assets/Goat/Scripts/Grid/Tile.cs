using System.Collections.Generic;
using UnityEngine;

namespace Goat.Grid
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
        None = -1,
        North = 0,
        East = 90,
        South = 180,
        West = 270
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
            wallPositions.Add(WallPosition.North, WallType.Empty);
            wallObjects.Add(WallPosition.North, null);
            wallPositions.Add(WallPosition.East, WallType.Empty);
            wallObjects.Add(WallPosition.East, null);
            wallPositions.Add(WallPosition.South, WallType.Empty);
            wallObjects.Add(WallPosition.South, null);
            wallPositions.Add(WallPosition.West, WallType.Empty);
            wallObjects.Add(WallPosition.West, null);
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
            if (floorObject) floorObject.SetActive(show);
        }
        public void ShowBuilding(bool show)
        {
            if (buildingObject) buildingObject.SetActive(show);
        }
        public void ShowWall(bool show, WallPosition position)
        {
            //code
            if(wallObjects[WallPosition.North]) wallObjects[WallPosition.North].SetActive(true);
            if(wallObjects[WallPosition.East]) wallObjects[WallPosition.East].SetActive(true);
            if(wallObjects[WallPosition.South]) wallObjects[WallPosition.South].SetActive(true);
            if(wallObjects[WallPosition.West]) wallObjects[WallPosition.West].SetActive(true);

            if(wallObjects[position]) wallObjects[position].SetActive(show);
            //check what wall to show
            //check what wall to hide
        }

        // Returns a tile info struct
        public TileInformation GetTileInformation()
        {
            // Initialize tile information
            return new TileInformation(centerPosition, floorType, buildingType, wallPositions);
        }

        // Change the tile type 
        // Spawns a floor object on this tile
        public void EditFloor(FloorType floorType, float rotationAngle)
        {
            Quaternion rotation = Quaternion.Euler(0, rotationAngle, 0);
            Vector3 size = Vector3.one * grid.GetTileSize;

            // If type and rotation remain the same as before
            if (this.floorType == floorType & floorObject && floorObject.transform.rotation == rotation && floorObject) return;
            // If rotation changes rotate object
            else if (this.floorType == floorType & floorObject && floorObject.transform.rotation != rotation) floorObject.transform.rotation = rotation;
            // If type changes instantiate new object
            else if (this.floorType != floorType)
            {
                if (floorObject) MonoBehaviour.Destroy(floorObject);
                if (floorType != FloorType.Empty)
                {
                    GameObject newObject = TileAssets.FindAsset(floorType);
                    floorObject = GameObject.Instantiate(newObject, centerPosition, rotation);
                    floorObject.transform.localScale = size;
                }
            }
            this.floorType = floorType;
        }

        // Change the building type
        // Spawns a building object on this tile
        public void EditBuilding(BuildingType buildingType, float rotationAngle)
        {
            Quaternion rotation = Quaternion.Euler(0, rotationAngle, 0);
            Vector3 size = Vector3.one * grid.GetTileSize;

            // If type and rotation remain the same as before
            if (this.buildingType == buildingType & buildingObject && buildingObject.transform.rotation == rotation) return;
            // If rotation changes rotate object
            else if (this.buildingType == buildingType & buildingObject && buildingObject.transform.rotation != rotation) buildingObject.transform.rotation = rotation;
            // If type changes instantiate new object
            else if (this.buildingType != buildingType)
            {
                if (buildingObject) MonoBehaviour.Destroy(buildingObject);
                if(buildingType != BuildingType.Empty)
                {
                    GameObject newObject = TileAssets.FindAsset(buildingType);
                    buildingObject = GameObject.Instantiate(newObject, centerPosition, rotation);
                    buildingObject.transform.localScale = size;
                }
            }
            this.buildingType = buildingType;
        }

        /// <summary>
        /// Edit wall at given position on tile
        /// </summary>
        /// <param name="position"> Position in degrees north=0, east=90, south=180, west=270 </param>
        /// <param name="type"> Type of wall to be placed at the given position </param>
        public void EditWall(WallType type, WallPosition position)
        {
            //Debug.Log("Old type: " + wallPositions[position].ToString() + " New type: " + type.ToString());
            // If walltype at position is a different type then the type yo be placed
            if (wallPositions[position] != type)
            {
                wallPositions[position] = type;
                // If walltype at position exists
                if (wallObjects[position]) MonoBehaviour.Destroy(wallObjects[position]);
                // Find gameobject
                GameObject newObject = TileAssets.FindAsset(type);
                // Instantiate gameobject
                if(type != WallType.Empty)
                {
                    Quaternion rotation = Quaternion.Euler(0, (float)(int)position, 0);
                    Vector3 size = Vector3.one * grid.GetTileSize;
                    wallObjects[position] = GameObject.Instantiate(newObject, centerPosition, rotation);
                    wallObjects[position].transform.localScale = size;
                }
            }
        }


    }

    // Structure for storing data
    public struct TileInformation
    {
        public readonly Vector3 TilePosition;
        public readonly FloorType floorType;
        public readonly BuildingType buildingType;
        public readonly Dictionary<WallPosition, WallType> walls;

        public TileInformation(Vector3 TilePosition, FloorType floorType, BuildingType buildingType, Dictionary<WallPosition, WallType> walls)
        {
            this.TilePosition = TilePosition;
            this.floorType = floorType;
            this.buildingType = buildingType;
            this.walls = walls;
        }
    }
}