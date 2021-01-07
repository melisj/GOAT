using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class StartGame : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private SceneLoaderForBuild sceneLoader;
    [SerializeField] private RectTransform[] menuButtons;
    private Sequence hideMenu;

    private void Awake()
    {
        startButton.onClick.AddListener(LoadGame);
    }

    private void LoadGame()
    {
        hideMenu = DOTween.Sequence();
        hideMenu.OnComplete(() => StartCoroutine(sceneLoader.LoadAllScenes(LoadComplete)));
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

    private void LoadComplete()
    {
        // Unload the first build scene
        SceneManager.UnloadSceneAsync(0);
    }
}