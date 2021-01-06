using System.Collections;
using System.Collections.Generic;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

public class TimeSetter : MonoBehaviour, IAtomListener<UnityAtoms.Void>
{
    [SerializeField] private Button timeButton;
    [SerializeField] private int timeScaleSet;
    [SerializeField] private int timeSpeedSet;
    [SerializeField] private IntEvent onTimeSpeedChanged;
    [SerializeField] private VoidEvent onTimeButtonClicked;
    [SerializeField] private Image timeIcon;
    [SerializeField] private Sprite activatedSprite, deactivatedSprite;
    [SerializeField] private bool active;

    public void OnEventRaised(Void item)
    {
        ChangeIcon();
    }

    private void ChangeIcon()
    {
        timeIcon.sprite = active ? activatedSprite : deactivatedSprite;
        active = !active && active;
    }

    private void Awake()
    {
        timeButton.onClick.AddListener(SetTime);
        ChangeIcon();
    }

    private void OnEnable()
    {
        onTimeButtonClicked.RegisterSafe(this);
    }

    private void OnDisable()
    {
        onTimeButtonClicked.UnregisterSafe(this);
    }

    private void SetTime()
    {
        onTimeButtonClicked.Raise();
        Time.timeScale = timeScaleSet;
        onTimeSpeedChanged.Raise(timeSpeedSet);
        active = true;
        ChangeIcon();
    }
}