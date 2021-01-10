using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Goat.Saving;

public class StartGame : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private RectTransform[] menuButtons;

    private const string START_SAVE = "DefaultSave/StartStore";

    private SceneLoaderForBuild sceneLoader;
    private Sequence hideMenu;

    private void Start()
    {
        sceneLoader = FindObjectOfType<SceneLoaderForBuild>();
        startButton.onClick.AddListener(() => LoadGame(START_SAVE));
    }

    public void LoadGame(string saveFile = "")
    {
        hideMenu = DOTween.Sequence();
        hideMenu.SetUpdate(true);
        hideMenu.OnComplete(() => StartCoroutine(sceneLoader.LoadAllScenes(() => LoadComplete(saveFile))));
        for (int i = 0; i < menuButtons.Length; i++)
        {
            hideMenu.Join(menuButtons[i].DOMove(menuButtons[i].position + (RandomMoveDirection() * (Screen.width)), 0.5f));
        }
    }

    private Vector3 RandomMoveDirection()
    {
        int random = Random.Range(0, 100);

        return random > 49 ? Vector3.right : -Vector3.right;
    }

    private void LoadComplete(string saveFile)
    {
        Debug.Log(saveFile);
        // Unload the first build scene
        if (SceneManager.GetSceneByBuildIndex(0).IsValid())
        {
            SceneManager.UnloadSceneAsync(0);
        }

        // Load the selected save file
        if (saveFile != "")
        {
            DataHandler dataHandler = FindObjectOfType<DataHandler>();
            dataHandler.LoadGame(saveFile);
        }
    }
}