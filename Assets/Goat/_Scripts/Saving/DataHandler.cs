using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEditor;

namespace Goat.Saving
{
    /// <summary>
    /// Data class which is serialized into the json file
    /// </summary>
    [Serializable]
    public class SaveData
    {
        private HashSet<SaveHandler> handlers = new HashSet<SaveHandler>();
        public List<DataContainer> data = new List<DataContainer>();

        public void AddData(SaveHandler container)
        {
            handlers.Add(container);
        }

        public void RemoveData(SaveHandler container)
        {
            handlers.Remove(container);
        }

        public void SetData()
        {
            data.Clear();
            IEnumerator<SaveHandler> enumarator = handlers.OrderBy((x) => x.saveOrder).GetEnumerator();
            while (enumarator.MoveNext())
            {
                data.Add(enumarator.Current.data);
            }
        }
    }

    /// <summary>
    /// Class for handling saving/loading the grid object.
    /// </summary>
    public class DataHandler : MonoBehaviour
    {
        public delegate void StartLoadLevelEvent(DataContainer data);
        public static event StartLoadLevelEvent StartLoadEvent;

        public delegate void StartSaveLevelEvent(SaveData data);
        public static event StartSaveLevelEvent StartSaveEvent;

        public delegate void LoadCompleteEvent();
        public static event LoadCompleteEvent LevelLoaded;

        JsonSerializerSettings jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

        private const string SAVE_FOLDER = "/SaveData/";
        private const string DEFAULT_STORE_PATH = "DefaultSave/StartStore";

        [Header("Paths")]
        [SerializeField] private string fileName = "DefaultSave";

        [InfoBox("Could not find file! This will create a file when saving.", InfoMessageType.Warning, "fileNotFound")]
        [SerializeField, ReadOnly] private string relativePath;
        private bool fileNotFound;

        private string folderPath;
        private string completePath;

        private void OnValidate() {
            SetPaths(fileName);
            fileNotFound = !File.Exists(completePath);
        }

        private string GetDefaultSave()
        {
            TextAsset asset = Resources.Load<TextAsset>(DEFAULT_STORE_PATH);
            return asset.text;
        }

        /// <summary>
        /// Save the current game status
        /// </summary>
        [Button("Save", ButtonSizes.Medium)]
        public void SaveGame() {
            if (Application.isPlaying)
            {
                SaveData data = new SaveData();
                StartSaveEvent.Invoke(data);
                data.SetData();

                SaveToFile(JsonConvert.SerializeObject(data, jsonSettings));
                Debug.LogFormat("Saved: {0} successfully", fileName);
            }
            else
                Debug.LogError("Editor needs to be playing to allow it to save!");
        }

        public void SaveGame(string saveFile)
        {
            SetPaths(saveFile);
            SaveGame();
        }

        [Button("Load", ButtonSizes.Medium)]
        public void LoadGame()
        {
            StartCoroutine(LoadGameCoroutine(false));
        }

        public void LoadGame(string saveFile, bool defaultSave)
        {
            SetPaths(saveFile);
            StartCoroutine(LoadGameCoroutine(defaultSave));
        }

        private void SetPaths(string fileName)
        {
            this.fileName = fileName;
            folderPath = string.Format("{0}{1}", Application.persistentDataPath, SAVE_FOLDER);
            relativePath = string.Format("{0}{1}.Json", SAVE_FOLDER, fileName);
            completePath = string.Format("{0}{1}", Application.persistentDataPath, relativePath);
        }

        /// <summary>
        /// Load the game file found in the path given in the inspector
        /// </summary>
        public IEnumerator LoadGameCoroutine(bool defaultSave) {
            if (Application.isPlaying)
            {
                string jsonData = defaultSave ? GetDefaultSave() : LoadFromFile();
                SaveData data = JsonConvert.DeserializeObject<SaveData>(jsonData, jsonSettings);

                if (data != null)
                {
                    foreach (DataContainer container in data.data)
                    {
                        StartLoadEvent.Invoke(container);
                        yield return null;
                    }

                    LevelLoaded?.Invoke();

                    Debug.LogFormat("Loaded: {0} successfully", fileName);
                }
                else
                {
                    Debug.LogWarning("Save file could not be read! Save might be empty/corrupted by changes in the SaveData class.");
                }
            }
            else
            {
                Debug.LogError("Editor needs to be playing to load a file!");
            }
        }

        /// <summary>
        /// Save the string to the given file
        /// </summary>
        /// <param name="json"> json string to be saved </param>
        private void SaveToFile(string json) {
            if (Directory.Exists(folderPath)) {
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

        public static string[] GetAllSaves()
        {
            if (!Directory.Exists(Application.persistentDataPath + SAVE_FOLDER))
            {
                Directory.CreateDirectory(Application.persistentDataPath + SAVE_FOLDER);
            }
            Debug.LogFormat("Loaded all saves in folder: {0}", Application.persistentDataPath + SAVE_FOLDER);
            return Directory.GetFiles(Application.persistentDataPath + SAVE_FOLDER, "*.json");
        }

        /// <summary>
        /// Clear the save file
        /// </summary>
        [Button("Clear File", ButtonSizes.Medium)]
        private void ClearData() {
#if UNITY_EDITOR
            if (File.Exists(completePath)) {
                if (EditorUtility.DisplayDialog("Clear save file?", string.Format("You will clear save file: {0}", completePath), "OK", "Cancel"))
                {
                    File.WriteAllText(completePath, "");
                    Debug.LogFormat("Cleared: {0} successfully", fileName);
                }
            }
            else {
                Debug.LogWarningFormat("Clearing could not be completed, path invalid: {0}", completePath);
            }
#endif
        }

        /// <summary>
        /// Clear the save file
        /// </summary>
        [Button("Delete File", ButtonSizes.Medium)]
        private void DeleteFIle()
        {
#if UNITY_EDITOR
            if (File.Exists(completePath))
            {
                if (EditorUtility.DisplayDialog("Delete save file?", string.Format("You will delete save file: {0}", completePath), "OK", "Cancel"))
                {
                    File.Delete(completePath);
                    Debug.LogFormat("Deletion: {0} successfully", fileName);
                }
            }
            else
            {
                Debug.LogWarningFormat("Deletion could not be completed, path invalid: {0}", completePath);
            }
#endif
        }
    }
}