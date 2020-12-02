using Goat.Grid.Interactions;
using Goat.Pooling;
using Goat.Storage;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Goat.Grid
{
    // Class for storing tile data
    public class Tile
    {
        private Vector3 centerPosition;
        private bool isUnlocked;
        private Placeable placeable;
        private GameObject floorObject, buildingObject, tileObject;
        private Grid grid;
        private GameObject[] wallObjs = new GameObject[4];

        public Vector3 Position => centerPosition;
        public TileInfo SaveData { get; set; }

        public Tile(Vector3 centerPosition, Vector2Int gridPosition, Grid grid)
        {
            this.centerPosition = centerPosition;
            this.grid = grid;
            SaveData = new TileInfo(gridPosition);
        }

        public void Reset() {
            for(int i =0; i < 4; i++) {
                if(wallObjs[i]) MonoBehaviour.Destroy(wallObjs[i]);
            }
            if(floorObject) MonoBehaviour.Destroy(floorObject);
            if (buildingObject) MonoBehaviour.Destroy(buildingObject);
            if (tileObject) MonoBehaviour.Destroy(tileObject);
            placeable = null;
        }

        public void ShowFloor(bool show)
        {
            if (floorObject) floorObject.SetActive(show);
        }

        public void ShowTile(bool show, float rotation = -1, Placeable placeable = null)
        {
            //Placeable is what you are going to be place
            //If (wallObjs is active) | buildingObj | floorObj
            if (this.placeable is Wall)
            {
                ShowAnyWall(show, rotation);
            }

            if (buildingObject)
            {
                buildingObject.SetActive(show ? show : placeable != null && !(placeable is Furniture));
            }

            if (floorObject)
            {
                floorObject.SetActive(show ? show : placeable != null && !(placeable is Floor));
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

        public void EditAny(Placeable placeable, float rotationAngle, bool destroyMode)
        {
            if (placeable is Wall)
            {
                EditAnyWall(placeable, rotationAngle, destroyMode);
                return;
            }
            Quaternion rotation = Quaternion.Euler(0, rotationAngle, 0);
            Vector3 size = Vector3.one * grid.GetTileSize;

            // Change rotation of existing placeable
            if (this.placeable == placeable & tileObject != null && tileObject.transform.rotation != rotation) {
                if(placeable is Floor) SaveData.SetFloor(placeable.ID, (int)rotationAngle); 
                else SaveData.SetBuilding(placeable.ID, (int)rotationAngle);

                tileObject.transform.rotation = rotation;
            }

            if ((tileObject && this.placeable == placeable) && !destroyMode)
            {
                return;
            }

            if (buildingObject && (!(placeable is Floor) | destroyMode))
            {   //Normally anything that is on the tile, e.g: if floor has nothing on it -> floor, if building is on it -> building
                //this.placeable.Amount++;

                SaveData.SetBuilding(-1, 0);
                MonoBehaviour.Destroy(buildingObject);
            }
            else if (floorObject && (!(placeable is Furniture) | destroyMode))
            {
                //So we deleted the building most likely, now it's time to delete the floor
                //this.placeable.Amount++;

                SaveData.SetFloor(-1, 0);
                MonoBehaviour.Destroy(floorObject);
            }
            if (placeable != null && !destroyMode)
            {
                GameObject newObject = placeable.Prefab;
                //placeable.Amount--;
                tileObject = GameObject.Instantiate(newObject, centerPosition, rotation);

                // tileObject = PoolManager.Instance.GetFromPool(newObject, centerPosition, rotation);
                tileObject.transform.localScale = size;
                if (placeable is Floor)
                {
                    SaveData.SetFloor(placeable.ID, (int)rotationAngle);
                    floorObject = tileObject;
                }
                else if (placeable is Furniture)
                {
                    SaveData.SetBuilding(placeable.ID, (int)rotationAngle);
                    buildingObject = tileObject;
                }
            }

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
                ///Debug.Log(index);
                //Debug.Log(rotationAngle);
                if (wallObjs[index]) {
                    MonoBehaviour.Destroy(wallObjs[index]);
                    SaveData.SetWall(-1, index);
                }

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

                    SaveData.SetWall(wall.ID, index);
                }
            }
        }

        public void LoadInData(TileInfo newData, ref List<Buyable> buyables) {
            SaveData = newData;

            int floorIndex = SaveData.GetFloor(out int floorRotation);
            if(floorIndex != -1)
                EditAny((Placeable)buyables[floorIndex], floorRotation, false);
 
            int buildingIndex = SaveData.GetBuilding(out int buildingRotation);
            if (buildingIndex != -1) {
                EditAny((Placeable)buyables[buildingIndex], buildingRotation, false);
                SaveData.LoadStorageData(buildingObject, ref buyables);
            }


            for (int i = 0; i < 4; i++) {
                int wallIndex = SaveData.GetWall(i);
                if (wallIndex != -1)
                    EditAnyWall((Placeable)buyables[wallIndex], (i * 90), false);
            }
        }

        public void SaveStorageData() {
            SaveData.SaveStorageData(buildingObject);
        }
    }

    [Serializable]
    public class TileInfo
    {
        public Vector2Int gridPosition;
        public int[] identifiers;
        public int[] rotations;
        public int[] storage;

        public TileInfo(Vector2Int gridPosition) {
            this.gridPosition = gridPosition;
            identifiers = new int[6] { -1, -1, -1, -1, -1, -1 };
            rotations = new int[2];
        }

        public void SaveStorageData(GameObject building) {
            if (building) {
                StorageInteractable interactable = building.GetComponentInChildren<StorageInteractable>();
                if (interactable) {
                    ItemInstance[] temp = interactable.PhysicalItemList;
                    storage = new int[temp.Length];
                    for (int i = 0; i < temp.Length; i++) {
                        storage[i] = temp[i] != null ? temp[i].Resource.ID : -1;
                    }
                }
            }
        }

        public void LoadStorageData(GameObject building, ref List<Buyable> buyables) {
            if (building && storage.Length != 0) {
                StorageInteractable interactable = building.GetComponentInChildren<StorageInteractable>();
                if (interactable) {
                    ItemInstance[] instanceList = new ItemInstance[storage.Length];
                    for (int i = 0; i < instanceList.Length; i++) {
                        if(storage[i] != -1)
                            instanceList[i] = new ItemInstance((Resource)buyables[storage[i]]);
                    }
                    interactable.PhysicalItemList = instanceList;

                }
            }
        }

        public void SetFloor(int ID, int rotation) {
            identifiers[0] = ID;
            rotations[0] = rotation;
        }

        public void SetBuilding(int ID, int rotation) {
            identifiers[1] = ID;
            rotations[1] = rotation;
        }

        public void SetWall(int ID, int rotation) {
            identifiers[rotation + 2] = ID;
        }


        public int GetFloor(out int rotation) {
            rotation = rotations[0];
            return identifiers[0];
        }

        public int GetBuilding(out int rotation) {
            rotation = rotations[1];
            return identifiers[1];
        }

        public int GetWall(int rotationIndex) {
            return identifiers[rotationIndex + 2];
        }
    }
}