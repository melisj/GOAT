using UnityEngine;
using TMPro;
using Goat.Events;
using System.Collections;

public class ChangeTimeText : EventListenerString
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private string tuie;

    public override void OnEventRaised(string value)
    {
        timeText.text = value;
    }
}