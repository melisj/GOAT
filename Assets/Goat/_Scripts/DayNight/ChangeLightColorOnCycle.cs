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
    [SerializeField, ColorPalette] private Color nightSkyboxColor;
    [SerializeField, ColorPalette] private Color daySkyboxColor;
    [SerializeField] private ParticleSystem daytimeParticle;
    [SerializeField] private ParticleSystem nighttimeParticle;

    public override void OnEventRaised(bool isday)
    {
        lightToChange.DOColor(isday ? dayColor : nightColor, transitionTime);
        RenderSettings.skybox.DOColor(isday ? daySkyboxColor : nightSkyboxColor, "_Tint", transitionTime);
        if (isday)
        {
            daytimeParticle.Play();
            nighttimeParticle.Stop();
        }
        else
        {
            daytimeParticle.Stop();
            nighttimeParticle.Play();
        }
            
    }
}