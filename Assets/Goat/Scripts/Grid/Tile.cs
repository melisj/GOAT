using Goat.Pooling;
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
        private Placeable placeable;
        private FloorType floorType;
        private GameObject floorObject, buildingObject, tileObject;
        private Grid grid;
        private Dictionary<WallPosition, WallType> wallPositions = new Dictionary<WallPosition, WallType>();
        private Dictionary<WallPosition, GameObject> wallObjects = new Dictionary<WallPosition, GameObject>();
        private GameObject[] wallObjs;

        public Tile(Vector3 centerPosition, Grid grid)
        {
            this.centerPosition = centerPosition;
            this.grid = grid;
            InitializeWalls();
        }

        private void InitializeWalls()
        {
            wallObjs = new GameObject[4];
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

        public void ShowTile(bool show, float rotation = -1)
        {
            if (rotation > -1)
            {
                ShowAnyWall(show, rotation);
            }

            if (tileObject)
            {
                tileObject.SetActive(show ? show : (this.placeable is Floor && placeable is Floor));
            }
        }

        public void ShowBuilding(bool show)
        {
            if (buildingObject) buildingObject.SetActive(show);
        }

        public void ShowAnyWall(bool show, float rotation)
        {
            for (int i = 0; i < wallObjs.Length; i++)
            {
                if (wallObjs[i] == null) continue;
                //if (i > 0)
                //{
                //    rotation += 90;
                //}
                wallObjs[i].SetActive(true);
            }
            int index = 0;
            if (rotation > 0)
            {
                index = (int)(rotation / 90);
            }
            if (wallObjs[index]) wallObjs[index].SetActive(show);
        }

        public void ShowWall(bool show, WallPosition position)
        {
            //code
            if (wallObjects[WallPosition.North]) wallObjects[WallPosition.North].SetActive(true);
            if (wallObjects[WallPosition.East]) wallObjects[WallPosition.East].SetActive(true);
            if (wallObjects[WallPosition.South]) wallObjects[WallPosition.South].SetActive(true);
            if (wallObjects[WallPosition.West]) wallObjects[WallPosition.West].SetActive(true);

            if (wallObjects[position]) wallObjects[position].SetActive(show);
            //check what wall to show
            //check what wall to hide
        }

        // Returns a tile info struct
        public TileInformation GetTileInformation()
        {
            // Initialize tile information
            return new TileInformation(centerPosition, floorType, buildingType, wallPositions, placeable);
        }

        public void EditAny(Placeable placeable, float rotationAngle, bool destroyMode)
        {
            if (placeable is Wall)
            {
                EditAnyWall(placeable, rotationAngle, destroyMode);
                return;
            }
            Quaternion rotation = Quaternion.Euler(0, rotationAngle, 0);
            Vector3 size = Vector3.one * grid.GetTileSize;

            // If type and rotation remain the same as before
            //  if (tileObject != null && tileObject.transform.rotation == rotation) return;
            // If rotation changes rotate object
            if (this.placeable == placeable & tileObject != null && tileObject.transform.rotation != rotation) tileObject.transform.rotation = rotation;
            // If type changes instantiate new object
            //else if (this.placeable != placeable)
            //{
            //  Debug.LogFormat("{0}, {1}:{2}, {3}:{4}", tileObject, this.placeable, this.placeable is Floor, placeable, placeable is Floor);
            if (tileObject && (this.placeable is Floor && placeable is Floor) | destroyMode)
            {   //Normally anything that is on the tile, e.g: if floor has nothing on it -> floor, if building is on it -> building
                MonoBehaviour.Destroy(tileObject);
            }
            else if (floorObject && destroyMode)
            {
                //So we deleted the building most likely, now it's time to delete the floor
                MonoBehaviour.Destroy(floorObject);
            }
            if (placeable != null && !destroyMode)
            {
                GameObject newObject = placeable.Prefab;
                placeable.Amount--;
                tileObject = GameObject.Instantiate(newObject, centerPosition, rotation);
                tileObject.transform.localScale = size;
                if (placeable is Floor)
                {
                    floorObject = tileObject;
                }
            }
            //}

            this.placeable = placeable;
        }

        public void EditAnyWall(Placeable wall, float rotationAngle, bool destroyMode)
        {
            if (this.placeable != wall)
            {
                // If walltype at position exists

                int index = 0;
                if (rotationAngle > 0)
                {
                    index = (int)(rotationAngle / 90);
                }
                if (wallObjs[index]) MonoBehaviour.Destroy(wallObjs[index]);
                // Find gameobject
                // Instantiate gameobject
                if (wall != null && !destroyMode)
                {
                    GameObject newObject = wall.Prefab;
                    Quaternion rotation = Quaternion.Euler(0, rotationAngle, 0);
                    //Quaternion newRotation = Quaternion.Euler(0, (float)(int)position, 0);
                    Vector3 size = Vector3.one * grid.GetTileSize;
                    wallObjs[index] = GameObject.Instantiate(newObject, centerPosition, rotation);
                    wallObjs[index].transform.localScale = size;
                }
            }
            //this.placeable = wall;
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
                if (buildingType != BuildingType.Empty)
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
                if (type != WallType.Empty)
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
        public readonly Placeable placeable;
        public readonly FloorType floorType;
        public readonly BuildingType buildingType;
        public readonly Dictionary<WallPosition, WallType> walls;

        public TileInformation(Vector3 TilePosition, FloorType floorType, BuildingType buildingType, Dictionary<WallPosition, WallType> walls, Placeable placeable)
        {
            this.TilePosition = TilePosition;
            this.floorType = floorType;
            this.buildingType = buildingType;
            this.walls = walls;
            this.placeable = placeable;
        }
    }
}