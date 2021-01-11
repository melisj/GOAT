using Goat.Farming;
using Goat.Grid.Interactions;
using Goat.Pooling;
using Goat.Storage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Grid
{
    // Class for storing tile data
    public class Tile
    {
        private Vector3 centerPosition;
        private Vector2Int gridPosition;
        private bool isUnlocked;
        private Placeable placeable;
        private GameObject floorObject, buildingObject, tileObject;
        private Grid grid;
        private GameObject[] wallObjs = new GameObject[4];
        private bool[] wallAuto = new bool[4];
        private int totalBeautyPoints;
        public GameObject FloorObj => floorObject;
        public GameObject[] WallObjs => wallObjs;
        public Vector3 Position => centerPosition;
        public Vector2Int TilePosition => gridPosition;
        public TileInfo SaveData { get; set; }

        // A tile is empty when does not have a building but does have a floor
        public bool IsEmpty => buildingObject == null && floorObject != null;

        public bool HasNoObjects => buildingObject == null && floorObject == null;

        public int TotalBeautyPoints => totalBeautyPoints;

        public bool HasWallOnSide(int rotation)
        {
            return wallObjs[rotation] != null;
        }

        public Tile(Vector3 centerPosition, Vector2Int gridPosition, Grid grid)
        {
            this.centerPosition = centerPosition;
            this.gridPosition = gridPosition;
            this.grid = grid;
            SaveData = new TileInfo(gridPosition);
        }

        public void Reset()
        {
            for (int i = 0; i < 4; i++)
            {
                if (wallObjs[i]) MonoBehaviour.Destroy(wallObjs[i]);
            }
            if (floorObject) MonoBehaviour.Destroy(floorObject);
            if (buildingObject) MonoBehaviour.Destroy(buildingObject);
            if (tileObject) MonoBehaviour.Destroy(tileObject);
            placeable = null;
        }

        public void ResetPooled()
        {
            for (int i = 0; i < 4; i++)
            {
                if (wallObjs[i]) PoolManager.Instance.ReturnToPool((wallObjs[i]));
                wallObjs[i] = null;
            }
            if (floorObject) PoolManager.Instance.ReturnToPool(floorObject);
            if (buildingObject) PoolManager.Instance.ReturnToPool(buildingObject);
            if (tileObject) PoolManager.Instance.ReturnToPool(tileObject);
            floorObject = null;
            buildingObject = null;
            tileObject = null;
            placeable = null;
            SaveData = new TileInfo(gridPosition);
        }

        public void ShowFloor(bool show)
        {
            if (floorObject) floorObject.SetActiveRenderer(show);
        }

        public void ShowTile(bool show, float rotation = -1, Placeable placeable = null)
        {
            //Placeable is what you are going to be place
            //If (wallObjs is active) | buildingObj | floorObj
            for (int i = 0; i < wallObjs.Length; i++)
            {
                if (wallObjs[i])
                    //if (placeable is Wall)
                    //{
                    ShowAnyWall(show, rotation, placeable);
                //}
            }

            if (buildingObject)
            {
                buildingObject.SetActiveRenderer(show ? show : placeable != null && !(placeable is Building));
            }

            if (floorObject)
            {
                floorObject.SetActiveRenderer(show ? show : placeable != null && !(placeable is Floor));
            }
        }

        public void ShowBuilding(bool show)
        {
            if (buildingObject) buildingObject.SetActiveRenderer(show);
        }

        public void ShowAnyWall(bool show, float rotation, Placeable placeable = null)
        {
            for (int i = 0; i < wallObjs.Length; i++)
            {
                if (wallObjs[i] == null) continue;

                wallObjs[i].SetActiveRenderer(true);
            }
            int index = 0;
            if (rotation > 0)
            {
                index = (int)(rotation / 90);
            }
            if (wallObjs[index]) wallObjs[index].SetActiveRenderer(show ? show : placeable != null && !(placeable is Wall));
        }

        public bool CheckForFloor(Placeable placeable, float rotationAngle = 0, bool destroyMode = false, bool isLoading = false)
        {
            if (destroyMode)
            {
                int objectsOnTile = 0;
                if (floorObject)
                    objectsOnTile++;
                if (buildingObject)
                    objectsOnTile++;
                for (int i = 0; i < wallObjs.Length; i++)
                {
                    if (wallObjs[i])
                        objectsOnTile++;
                }
                return objectsOnTile == 0;
            }

            if (placeable && !isLoading)
            {
                if (placeable.CanBuy(1) && !destroyMode)
                    return true;
            }

            if (placeable is Wall)
            {
                int index = 0;

                if (rotationAngle > 0)
                {
                    index = (int)(rotationAngle / 90);
                }

                if (wallObjs[index] && wallAuto[index] && destroyMode) return true;
            }

            if (placeable is FarmStation)
            {
                return CheckForResourceTile(placeable);
            }

            if (placeable is Building building)
            {
                if (building.PlaceableWithoutFloor)
                    return false;
            }

            return ((!floorObject && !(placeable is Floor)));
        }

        private bool CheckForResourceTile(Placeable placeable)
        {
            //FloorObj getComp.
            if (floorObject)
            {
                ResourceTile resourceTile = floorObject.GetComponent<ResourceTile>();
                if (!resourceTile) return true;
                Resource resOnTile = resourceTile.Data.Resource;

                FarmStation station = (FarmStation)placeable;
                for (int i = 0; i < station.ResourceFarms.Length; i++)
                {
                    if (station.ResourceFarms[i] == resOnTile)
                        return false;
                }
            }
            return true;
            //if Placeable is FarmStation -> Check for resource

            //if FloorObj Resourceblabla == FarmStation.resource
            //OK!
        }

        public PlaceableInfo GetPlaceableInfo()
        {
            if (!floorObject) return null;

            PlaceableInfo placeableInfo = floorObject.GetComponent<PlaceableInfo>();
            return placeableInfo;
        }

        public bool EditAny(Placeable placeable, float rotationAngle, bool destroyMode, bool isLoading = false)
        {
            //Stop editing immediately if you want to place anything (excl. a new floor) on a floor that doesn't exist

            if (CheckForFloor(placeable, rotationAngle, destroyMode, isLoading)) { return false; }

            if (placeable is Wall)
            {
                return EditAnyWall(placeable, rotationAngle, destroyMode, isLoading);
            }

            if (destroyMode)
            {
                if (buildingObject)
                    Destroy(ref buildingObject, isLoading);
                else if (floorObject)
                    Destroy(ref floorObject, isLoading);
            }

            Quaternion rotation = Quaternion.Euler(0, rotationAngle, 0);
            Vector3 size = Vector3.one * grid.GetTileSize;
            GameObject tempTile = null;
            // Change rotation of existing placeable
            if (this.placeable == placeable & tileObject != null && tileObject.transform.rotation != rotation)
            {
                if (placeable is Floor) SaveData.SetFloor(placeable.ID, (int)rotationAngle);
                else SaveData.SetBuilding(placeable.ID, (int)rotationAngle);

                tileObject.transform.rotation = rotation;
            }
            if ((tileObject && this.placeable == placeable) && !destroyMode)
            {
                //No need to destroy if you want to edit the same tile with the same type
                return false;
            }

            DestroyTile(placeable, isLoading);
            if (placeable != null && !destroyMode)
            {
                GameObject newObject = placeable.Prefab;
                //placeable.Amount--;
                // tileObject = GameObject.Instantiate(newObject, centerPosition, rotation);

                tileObject = PoolManager.Instance.GetFromPool(newObject, centerPosition, rotation);
                PlaceableInfo placeableInfo = tileObject.GetComponent<PlaceableInfo>();
                placeableInfo.Setup(placeable);
                MeshFilter[] tileObjectFilter = tileObject.GetComponentsInChildren<MeshFilter>();
                for (int i = 0; i < tileObjectFilter.Length; i++)
                {
                    if (i >= placeable.Mesh.Length)
                    {
                        tileObjectFilter[i].mesh = null;
                        continue;
                    }
                    tileObjectFilter[i].mesh = placeable.Mesh[i];
                }

                // Change the grid position this object is on
                BaseInteractable interactable = tileObject.GetComponentInChildren<BaseInteractable>();
                if (interactable)
                {
                    interactable.GridPosition = gridPosition;
                    interactable.CenterPosition = centerPosition;
                    interactable.grid = grid;
                }

                tileObject.transform.localScale = size;
                if (placeable is Floor)
                {
                    SaveData.SetFloor(placeable.ID, (int)rotationAngle);
                    floorObject = tileObject;
                    SetupResourceTile(floorObject, placeable);
                    tempTile = floorObject;
                }
                else if (placeable is Building)
                {
                    SaveData.SetBuilding(placeable.ID, (int)rotationAngle);
                    buildingObject = tileObject;
                    tempTile = buildingObject;
                    if (GetWallObj(InverseRotation((int)rotationAngle)))
                        AdjustPositionAgainstWall(buildingObject);
                }
                totalBeautyPoints += placeable.BeautyPoints;
                if (!isLoading)
                    placeable.Buy(1);
            }

            this.placeable = placeable;
            return tempTile != null | destroyMode;
        }

        private void DestroyTile(Placeable placeable, bool isLoading)
        {
            if (buildingObject && (!(placeable is Floor)))
            {   //Normally anything that is on the tile, e.g: if floor has nothing on it -> floor, if building is on it -> building
                Destroy(ref buildingObject, isLoading);
            }
            else if (floorObject && (!(placeable is Building)))
            {
                //So we deleted the building most likely, now it's time to delete the floor
                //this.placeable.Amount++;
                Destroy(ref floorObject, isLoading);
            }
        }

        private void Destroy(ref GameObject objectToDestroy, bool isLoading)
        {
            PlaceableInfo placeableInfo = objectToDestroy.GetComponent<PlaceableInfo>();
            SaveData.SetBuilding(-1, 0);
            PoolManager.Instance.ReturnToPool(objectToDestroy);
            if (objectToDestroy == tileObject)
            {
                tileObject = null;
            }
            objectToDestroy = null;

            if (!isLoading)
                placeableInfo.Placeable.Sell(1);
            Debug.Log("Destroying tiles");
            totalBeautyPoints -= placeableInfo.Placeable.BeautyPoints;
            placeableInfo.Setup(null);
        }

        //Detect tile has neighbouring tiles
        //if no neighbouring tiles, add wall in that direction
        //if neighbouring tiles, delete wall there
        public bool EditAnyWall(Placeable wall, float rotationAngle, bool destroyMode, bool autoMode = false, bool isLoading = false)
        {
            //if (this.placeable != wall)
            //{
            // If walltype at position exists

            int index = 0;
            MeshFilter[] tileObjectFilter = null;
            if (rotationAngle > 0)
            {
                index = (int)(rotationAngle / 90);
            }

            if (wallObjs[index] && !destroyMode)
            {
                tileObjectFilter = wallObjs[index].GetComponentsInChildren<MeshFilter>();
                for (int j = 0; j < wall.Mesh.Length; j++)
                {
                    for (int i = 0; i < tileObjectFilter.Length; i++)
                    {
                        if (!autoMode && tileObjectFilter[i].sharedMesh == wall.Mesh[j])
                        {
                            return false;
                        }
                    }
                }
            }

            if (wallObjs[index])
            {
                //if ((autoMode))
                //{
                //MeshFilter[] tileObjectFilter = wallObjs[index].GetComponentsInChildren<MeshFilter>();
                //for (int i = 0; i < tileObjectFilter.Length; i++)
                //{
                //    if (!autoMode && tileObjectFilter[i].sharedMesh != wall.Mesh[i])
                //    {
                //        tileObjectFilter[i].mesh = null;
                //    }
                //}
                DestroyWall(rotationAngle, autoMode, isLoading);
                //  }
            }

            if (wall != null && !destroyMode)
            {
                GameObject newObject = wall.Prefab;
                Quaternion rotation = Quaternion.Euler(0, rotationAngle, 0);
                Vector3 size = Vector3.one * grid.GetTileSize;
                //   wallObjs[index] = GameObject.Instantiate(newObject, centerPosition, rotation);
                //if (!wallObjs[index])
                wallObjs[index] = PoolManager.Instance.GetFromPool(newObject, centerPosition, rotation);
                PlaceableInfo placeableInfo = wallObjs[index].GetComponent<PlaceableInfo>();
                placeableInfo.Setup(wall);
                wallObjs[index].transform.localScale = size;
                if (autoMode)
                    wallAuto[index] = true;

                tileObjectFilter = wallObjs[index].GetComponentsInChildren<MeshFilter>();
                for (int i = 0; i < tileObjectFilter.Length; i++)
                {
                    if (i >= wall.Mesh.Length)
                    {
                        tileObjectFilter[i].mesh = null;
                        continue;
                    }
                    tileObjectFilter[i].mesh = wall.Mesh[i];
                }
                if (!autoMode && !isLoading)
                    placeableInfo.Placeable.Buy(1);

                totalBeautyPoints += placeableInfo.Placeable.BeautyPoints;
                if (buildingObject && (int)buildingObject.transform.eulerAngles.y == InverseRotation((int)rotationAngle))
                    AdjustPositionAgainstWall(buildingObject);
                SaveData.SetWall(wall.ID, index, autoMode);
            }
            // }
            return true;
        }

        private void DestroyWall(float rotationAngle, bool autoMode, bool isLoading)
        {
            int index = GetWallIndex(rotationAngle);

            PlaceableInfo placeableInfo = wallObjs[index].GetComponent<PlaceableInfo>();
            Debug.Log("Destroying");
            if (!autoMode && !isLoading)
                placeableInfo.Placeable.Sell(1);

            totalBeautyPoints -= placeableInfo.Placeable.BeautyPoints;
            PoolManager.Instance.ReturnToPool(wallObjs[index]);
            wallObjs[index] = null;
            wallAuto[index] = autoMode ? false : wallAuto[index];
            placeableInfo.Setup(null);
            SaveData.SetWall(-1, index, false);
            if (buildingObject && (int)buildingObject.transform.eulerAngles.y == InverseRotation((int)rotationAngle))
                AdjustPositionAgainstWall(buildingObject, true);
        }

        private int GetWallIndex(float rotationAngle)
        {
            int index = 0;
            if (rotationAngle > 0)
            {
                index = (int)(rotationAngle / 90);
            }
            return index;
        }

        private void AdjustPositionAgainstWall(GameObject building, bool reset = false)
        {
            AdjustPositionAgainstWall adjustPos = building.GetComponent<AdjustPositionAgainstWall>();
            if (!adjustPos) return;
            if (!reset)
                adjustPos.AdjustPosition();
            else
                adjustPos.ResetPosition();
        }

        private GameObject GetWallObj(float rotationAngle)
        {
            int index = 0;
            if (rotationAngle > 0)
            {
                index = (int)(rotationAngle / 90);
            }
            return WallObjs[index];
        }

        private int InverseRotation(int rotation)
        {
            return (rotation + 180) % 360;
        }

        public void LoadInData(TileInfo newData, ref GridObjectsList objectList)
        {
            SaveData = newData;

            int floorIndex = SaveData.GetFloor(out int floorRotation);
            if (floorIndex != -1 && objectList.GetObject(floorIndex) is Placeable)
                EditAny((Placeable)objectList.GetObject(floorIndex), floorRotation, false, true);

            int buildingIndex = SaveData.GetBuilding(out int buildingRotation);
            if (buildingIndex != -1 && objectList.GetObject(buildingIndex) is Placeable)
            {
                EditAny((Placeable)objectList.GetObject(buildingIndex), buildingRotation, false, true);
                SaveData.LoadStorageData(buildingObject, ref objectList);
            }

            for (int i = 0; i < 4; i++)
            {
                int wallIndex = SaveData.GetWall(i, out bool isAutoWall);
                if (wallIndex != -1 && objectList.GetObject(wallIndex) is Placeable)
                    EditAnyWall((Placeable)objectList.GetObject(wallIndex), (i * 90), false, isAutoWall, true);
            }
        }

        private void SetupResourceTile(GameObject obj, Placeable placeable)
        {
            if (placeable is ResourceTileData resFloor)
            {
                ResourceTile resTile = obj.GetComponent<ResourceTile>();
                if (!resTile) return;
                resTile.Setup(resFloor);
            }
        }

        public void SaveStorageData()
        {
            SaveData.SaveStorageData(buildingObject);
        }
    }
}

[Serializable]
public class TileInfo
{
    public Vector2Int gridPosition;
    public int[] identifiers;
    public int[] rotations;
    public bool[] wallAuto;
    public string storage;

    public TileInfo(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
        identifiers = new int[6] { -1, -1, -1, -1, -1, -1 };
        rotations = new int[2];
        wallAuto = new bool[4];
        storage = "";
    }

    public void SaveStorageData(GameObject building)
    {
        if (building)
        {
            StorageInteractable interactable = building.GetComponentInChildren<StorageInteractable>();
            if (interactable)
                storage = interactable.Inventory.Save();
        }
    }

    public void LoadStorageData(GameObject building, ref GridObjectsList objectList)
    {
        if (building && storage.Length != 0)
        {
            StorageInteractable interactable = building.GetComponentInChildren<StorageInteractable>();
            if (interactable)
                interactable.Inventory.Load(storage, objectList);
        }
    }

    public void SetFloor(int ID, int rotation)
    {
        identifiers[0] = ID;
        rotations[0] = rotation;
    }

    public void SetBuilding(int ID, int rotation)
    {
        identifiers[1] = ID;
        rotations[1] = rotation;
    }

    public void SetWall(int ID, int rotation, bool wallAuto)
    {
        identifiers[rotation + 2] = ID;
        this.wallAuto[rotation] = wallAuto;
    }

    public int GetFloor(out int rotation)
    {
        rotation = rotations[0];
        return identifiers[0];
    }

    public int GetBuilding(out int rotation)
    {
        rotation = rotations[1];
        return identifiers[1];
    }

    public int GetWall(int rotationIndex, out bool isAutoWall)
    {
        isAutoWall = wallAuto[rotationIndex];
        return identifiers[rotationIndex + 2];
    }
}