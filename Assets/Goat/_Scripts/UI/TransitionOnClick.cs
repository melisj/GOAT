using Goat.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionOnClick : MonoBehaviour
{
    [SerializeField] private GameplayTransition gameplayTransition;
    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(gameplayTransition.Transition);
    }
}