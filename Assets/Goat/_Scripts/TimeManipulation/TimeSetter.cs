using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSetter : MonoBehaviour
{
    [SerializeField] private Button timeButton;
    [SerializeField] private int timeSet;

    private void Awake()
    {
        timeButton.onClick.AddListener(SetTime);
    }

    private void SetTime()
    {
        Time.timeScale = timeSet;
    }
}