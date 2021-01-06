using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private SceneLoaderForBuild sceneLoader;

    private void Awake()
    {
        startButton.onClick.AddListener(LoadGame);
    }

    private void LoadGame()
    {
        StartCoroutine(sceneLoader.LoadAllScenes(LoadComplete));
    }

    private void LoadComplete()
    {
        // Unload the first build scene
        SceneManager.UnloadSceneAsync(0);
    }
}
