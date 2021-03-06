﻿using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderForBuild : MonoBehaviour
{
    [SerializeField, ReadOnly] private string[] scenePaths;
    private static SceneLoaderForBuild instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
    }

    public void SetPaths(string[] scenePaths)
    {
        this.scenePaths = scenePaths;
    }

    public IEnumerator LoadAllScenes(Action callback)
    {
        foreach (string path in scenePaths)
        {
            if (!SceneManager.GetSceneByPath(path).IsValid())
            {
                AsyncOperation operation = SceneManager.LoadSceneAsync(path, LoadSceneMode.Additive);
                yield return new WaitUntil(() => operation.isDone);
            }
        }
        callback.Invoke();
    }

    [Button("Empty list")]
    public void EmptyList()
    {
        scenePaths = null;
    }
}
