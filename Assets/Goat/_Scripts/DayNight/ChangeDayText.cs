using UnityEngine;
using TMPro;
using Goat.Events;

public class ChangeDayText : EventListenerInt
{
    [SerializeField] private TextMeshProUGUI dayText;

    public override void OnEventRaised(int value)
    {
        dayText.text = "Day " + value.ToString();
    }
}