using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;

namespace Goat.Saving
{
    public class TimeSaveHandler : SaveHandler
    {
        public TimeOfDay time;
        public BoolEvent OnCycleChange;
        public IntEvent OnDayChanged;

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
        public long ticks;

        public override IEnumerator Load(SaveHandler handler)
        {
            TimeSaveHandler timeHandler = (TimeSaveHandler)handler;

            timeHandler.time.CurrentDay = day;
            timeHandler.time.TimeOfDayMinutes = timeSec;
            timeHandler.time.TimeOfDay12Hours = timeHour % 12;
            timeHandler.time.TimeOfDayHours = timeHour;
            timeHandler.time.Date = new DateTime(ticks);

            bool isDay = timeHour >= timeHandler.time.TimeOfSunrise && timeHour < timeHandler.time.TimeOfSunset;

            timeHandler.OnDayChanged.Raise(timeHandler.time.Date.Day);
            timeHandler.OnCycleChange.Raise(isDay);

            DoneLoading(handler, DataHandler.ContainerExitCode.Success);
            yield break;
        }

        public override void Save(SaveHandler handler)
        {
            TimeSaveHandler timeHandler = (TimeSaveHandler)handler;

            day = timeHandler.time.CurrentDay;
            timeSec = timeHandler.time.TimeOfDayMinutes;
            timeHour = timeHandler.time.TimeOfDayHours;
            ticks = timeHandler.time.Date.Ticks;
        }
    }
}
