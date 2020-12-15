using UnityEngine;
using TMPro;
using Goat.Events;

public class ChangeTimeText : EventListenerString
{
    [SerializeField] private TextMeshProUGUI timeText;

    public override void OnEventRaised(string value)
    {
        timeText.text = value;
    }
}