using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Goat.Grid
{
    /// <summary>
    /// Data class which is serialized into the json file
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public List<TileInfo> tileList = new List<TileInfo>();

        // Set the save data a list
        public SaveData(Tile[,] tiles) {
            for (int x = 0; x < tiles.GetLength(0); x++) {
                for (int y = 0; y < tiles.GetLength(1); y++) {
                    tiles[x, y].SaveStorageData();
                    tileList.Add(tiles[x, y].SaveData);
                }
            }
        }
    }

    /// <summary>
    /// Class for handling saving/loading the grid object.
    /// </summary>
    public class GridDataHandler : MonoBehaviour
    {

        [Header("Paths")]
        [SerializeField] private string saveFolder = "/Goat/SaveData/";
        [SerializeField] private string fileName = "GridSave";
        private string completePath;

        private Grid grid;

        private void Awake() {
            grid = GetComponent<Grid>();
        }

        private void OnValidate() {
            completePath = string.Format("{0}{1}{2}.Json", Application.dataPath, saveFolder, fileName);
        }

        /// <summary>
        /// Save the current grid status
        /// </summary>
        [Button("Save", ButtonSizes.Medium)]
        public void SaveGrid() {
            if (Application.isPlaying)
                SaveToFile(JsonUtility.ToJson(new SaveData(grid.tiles)));
            else
                Debug.LogError("Editor needs to be playing to allow it to save");
        }

        /// <summary>
        /// Load the grid found in the path given in the inspector
        /// </summary>
        [Button("Load", ButtonSizes.Medium)]
        public void LoadGrid() {
            if (Application.isPlaying) {
                grid.Reset();
                SaveData data = JsonUtility.FromJson<SaveData>(LoadFromFile());

                if (data != null) {
                    // Load references for the data
                    List<Buyable> buyables = Resources.LoadAll<Buyable>("").ToList();
                    buyables = buyables.OrderBy((obj) => obj.ID).ToList();

                    // Load in all the data on the tiles
                    for (int x = 0; x < grid.GetGridSize.x; x++) {
                        for (int y = 0; y < grid.GetGridSize.y; y++) {
                            grid.tiles[x, y].LoadInData(data.tileList[grid.GetGridSize.y * x + y], ref buyables);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save the string to the given file
        /// </summary>
        /// <param name="json"> json string to be saved </param>
        private void SaveToFile(string json) {
            if (Directory.Exists(Application.dataPath + saveFolder)) {
                File.WriteAllText(completePath, json);
            } else {
                Debug.LogWarningFormat("Saving could not be completed, path invalid: {0}", completePath);
            }
        }

        /// <summary>
        /// Loads json file and return the json string
        /// </summary>
        /// <returns> returns the json string </returns>
        private string LoadFromFile() {
            string json = "";
            if (File.Exists(completePath)) {
                json = File.ReadAllText(completePath);
            } else {
                Debug.LogWarningFormat("Loading could not be completed, path invalid: {0}", completePath);
            }

            return json;
        }


        /// <summary>
        /// Clear the save file
        /// </summary>
        /// <param name="json"></param>
        [Button("Clear File", ButtonSizes.Medium)]
        private void ClearData() {
            if (File.Exists(completePath)) {
                File.WriteAllText(completePath, "");
            } else {
                Debug.LogWarningFormat("Clearing could not be completed, path invalid: {0}", completePath);
            }
        }
    }
}