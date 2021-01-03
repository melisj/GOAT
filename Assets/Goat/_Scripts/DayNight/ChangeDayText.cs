using UnityEngine;
using TMPro;
using Goat.Events;
using System;
using System.Globalization;

public class ChangeDayText : EventListenerInt
{
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TimeOfDay timeOfDay;

    private void Start()
    {
        dayText.text = timeOfDay.GetDate();
    }

    public override void OnEventRaised(int value)
    {
        //dayText.text = "Day " + value.ToString();
        dayText.text = timeOfDay.GetDate();
    }
}