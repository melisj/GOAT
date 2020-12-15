using UnityEngine;
using Sirenix.OdinInspector;
using Goat.Events;
using DG.Tweening;

public class ChangeLightColorOnCycle : EventListenerBool
{
    [SerializeField] private Light lightToChange;
    [SerializeField] private int transitionTime;
    [SerializeField, ColorPalette] private Color nightColor;
    [SerializeField, ColorPalette] private Color dayColor;

    public override void OnEventRaised(bool isday)
    {
        lightToChange.DOColor(isday ? dayColor : nightColor, transitionTime);
    }
}