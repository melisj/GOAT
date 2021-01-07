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

        [Header("Paths")]
        [SerializeField] private string saveFolder = "/Goat/SaveData/";
        [SerializeField] private string fileName = "DefaultSave";

        [InfoBox("Could not find file! This will create a file when saving.", InfoMessageType.Warning, "fileNotFound")]
        [SerializeField] private string relativePath;
        private bool fileNotFound;

        private string completePath;

        private void OnValidate() {
            relativePath = string.Format("{0}{1}.Json", saveFolder, fileName);
            completePath = string.Format("{0}{1}", Application.dataPath, relativePath);
            fileNotFound = !File.Exists(completePath);
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

        [Button("Load", ButtonSizes.Medium)]
        public void LoadGame()
        {
            StartCoroutine(LoadGameCoroutine());
        }

        /// <summary>
        /// Load the game file found in the path given in the inspector
        /// </summary>
        public IEnumerator LoadGameCoroutine() {
            if (Application.isPlaying)
            {
                SaveData data = JsonConvert.DeserializeObject<SaveData>(LoadFromFile(), jsonSettings);

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
#if UNITY_EDITOR
            if (File.Exists(completePath)) {
                if (EditorUtility.DisplayDialog("Delete save file?", string.Format("You will delete save file: {0}", completePath), "OK", "Cancel"))
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
    }
}