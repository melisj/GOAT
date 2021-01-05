using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderForBuild : MonoBehaviour
{
    [SerializeField, ReadOnly] private string[] scenePaths;

    public void SetPaths(string[] scenePaths)
    {
        this.scenePaths = scenePaths;
    }

    public void LoadAllScenes()
    {
        foreach(string path in scenePaths) 
        {
            SceneManager.LoadSceneAsync(path, LoadSceneMode.Additive);
        }
    }

    [Button("Empty list")]
    public void EmptyList()
    {
        scenePaths = null;
    }
}
