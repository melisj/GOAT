﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityAtoms.BaseAtoms;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Globalization;
using UnityAtoms;

public partial class DayNightCycle : MonoBehaviour, IAtomListener<int>
{
    [SerializeField] private TimeOfDay timeOfDay;

    //transition timer for lerping the time of day
    public bool isDay;
    //current hours and day

    //which hour of day the sun rises and sets

    //the regular speed of the day + clock. not to be confused with time manipulation
    [SerializeField] private BoolEvent OnChangeCycle;
    [SerializeField] private IntEvent onTimeSpeedChanged;
    [SerializeField, ProgressBar(1, 10)] private int timeScale;

    //Events for OnDayTime and OnNightTime
    //public event EventHandler<bool> OnChangeCycle;
    private void OnEnable()
    {
        onTimeSpeedChanged.RegisterSafe(this);
    }

    private void OnDisable()
    {
        onTimeSpeedChanged.UnregisterSafe(this);
    }

    public void OnEventRaised(int changedTimeSpeed)
    {
        timeOfDay.TimeScale = changedTimeSpeed;
    }

    private void Start()
    {
        SetTimeNight();
        Time.timeScale = timeScale;
    }

    private void Update()
    {
        UpdateClock();
    }

    private void UpdateClock()
    {
        timeOfDay.TimeOfDayMinutes += Time.deltaTime * timeOfDay.TimeScale;

        if (timeOfDay.TimeOfDayMinutes >= 60)
        {
            timeOfDay.TimeOfDayHours++;
            timeOfDay.TimeOfDay12Hours = timeOfDay.TimeOfDayHours % 12;

            timeOfDay.TimeOfDayMinutes = 0;
            
            //check if its morning, nighttime or midnight
            if (timeOfDay.TimeOfDayHours == timeOfDay.TimeOfSunrise)
            {
                SetTimeDay();
            }
            else if (timeOfDay.TimeOfDayHours == timeOfDay.TimeOfSunset)
            {
                SetTimeNight();
            }
            else if (timeOfDay.TimeOfDayHours == 24)
            {
                timeOfDay.CurrentDay++;
                timeOfDay.TimeOfDayHours = 0;
            }
        }
    }

    private void SetTimeDay()
    {
        //from nighttime to daytime
        isDay = true;
        OnChangeCycle.Raise(isDay);
    }

    private void SetTimeNight()
    {
        //from daytime to nighttime
        isDay = false;
        OnChangeCycle.Raise(isDay);
    }
}