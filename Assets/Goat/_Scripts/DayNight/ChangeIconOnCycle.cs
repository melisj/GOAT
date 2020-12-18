using UnityEngine;
using UnityEngine.UI;
using Goat.Events;

public class ChangeIconOnCycle : EventListenerBool
{
    [SerializeField] private Image dayNightIcon;
    [SerializeField] private Sprite moon;
    [SerializeField] private Sprite sun;

    public override void OnEventRaised(bool isday)
    {
        dayNightIcon.sprite = isday ? sun : moon;
    }
}