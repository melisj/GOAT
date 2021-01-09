using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvokeOtherButtonOnClick : MonoBehaviour
{
    [SerializeField] private Button thisButton;
    [SerializeField] private Button otherButton;

    private void Awake()
    {
        thisButton.onClick.AddListener(otherButton.onClick.Invoke);
    }
}