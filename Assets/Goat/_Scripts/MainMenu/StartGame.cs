using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Goat.Saving;
using Sirenix.OdinInspector;

public class StartGame : MonoBehaviour
{
    [SerializeField] private bool startFreshButton;
    [SerializeField, ShowIf("startFreshButton")] private Button startButton;
    [SerializeField] private RectTransform[] menuButtons;

    private SceneLoaderForBuild sceneLoader;
    private Sequence hideMenu;

    private void Start()
    {
        sceneLoader = FindObjectOfType<SceneLoaderForBuild>();
        if(startFreshButton)
            startButton.onClick.AddListener(() => LoadGame("", true));
    }

    public void LoadGame(string saveFile = "", bool defaultSave = false)
    {
        hideMenu = DOTween.Sequence();
        hideMenu.SetUpdate(true);
        hideMenu.OnComplete(() => StartCoroutine(sceneLoader.LoadAllScenes(() => LoadComplete(saveFile, defaultSave))));
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

    private void LoadComplete(string saveFile, bool defaultSave)
    {
        // Unload the first build scene
        if (SceneManager.GetSceneByBuildIndex(0).IsValid())
        {
            SceneManager.UnloadSceneAsync(0);
        }

        // Load the selected save file
        if (saveFile != "" || defaultSave)
        {
            DataHandler dataHandler = FindObjectOfType<DataHandler>();
            dataHandler.LoadGame(saveFile, defaultSave);
        }
    }
}