﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Saving
{
    public class TimeSaveHandler : SaveHandler
    {
        public TimeOfDay time;
        public ChangeLightColorOnCycle changeLightHandler;
        public ChangeIconOnCycle changeIconHandler;

        private void Awake()
        {
            data = new TimeSaveData();
        }
    }

    public class TimeSaveData : DataContainer, ISaveable
    {
        public int day;
        public float timeSec;
        public int timeHour;

        public override void Load(SaveHandler handler)
        {
            TimeSaveHandler timeHandler = (TimeSaveHandler)handler;

            timeHandler.time.CurrentDay = day;
            timeHandler.time.TimeOfDayMinutes = timeSec;
            timeHandler.time.TimeOfDay12Hours = timeHour % 12;
            timeHandler.time.TimeOfDayHours = timeHour;

            bool isDay = timeHour > timeHandler.time.TimeOfSunrise && timeHour < timeHandler.time.TimeOfSunset;
            timeHandler.changeLightHandler.OnEventRaised(isDay);
            timeHandler.changeIconHandler.OnEventRaised(isDay);
        }

        public override void Save(SaveHandler handler)
        {
            TimeSaveHandler timeHandler = (TimeSaveHandler)handler;

            day = timeHandler.time.CurrentDay;
            timeSec = timeHandler.time.TimeOfDayMinutes;
            timeHour = timeHandler.time.TimeOfDayHours;
        }
    }
}
