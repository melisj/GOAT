using UnityEngine;
using TMPro;
using Goat.Events;
using UnityAtoms;

public class ChangeTimeText : EventListenerVoid
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TimeOfDay time;

    public override void OnEventRaised(Void value)
    {
        timeText.text = time.GetTime12Hour;
    }
}