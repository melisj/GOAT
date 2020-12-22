using UnityEngine;
using UnityAtoms.BaseAtoms;
using Sirenix.OdinInspector;
using System;
using System.Globalization;
using System.Threading;

[CreateAssetMenu(fileName = "Time", menuName = "ScriptableObjects/GlobalVariables/Time")]
public class TimeOfDay : ScriptableObject
{
    private readonly string AM = "AM";
    private readonly string PM = "PM";

    [Title("Current Time")]
    [SerializeField, ReadOnly] private int timeOfDayHours;
    [SerializeField, ReadOnly] private int timeOfDay12Hours;
    [SerializeField, ReadOnly] private int currentDay;
    [SerializeField, ReadOnly] private float timeOfDayMinutes;
    [SerializeField, ReadOnly] private string englishTime;

    [SerializeField, ProgressBar(0, 24, r: 1f, g: 0.9f, b: 0.7f)] private int timeOfSunrise = 8;
    [SerializeField, ProgressBar(0, 24, r: 0.5f, g: 0.4f, b: 0.6f)] private int timeOfSunset = 17;
    [SerializeField] private int currentYear;
    [SerializeField, Range(1, 12)] private int currentMonth;

    [Title("Events")]
    [SerializeField] private StringEvent onTime24Changed;
    [SerializeField] private StringEvent onTime12Changed;
    [SerializeField] private IntEvent onDayChanged;
    [SerializeField] private IntEvent onMonthChanged;
    private DateTime date;

    public string EnglishTime => englishTime = timeOfDayHours / 12 >= 1 ? PM : AM;
    public string GetTime12Hour => $"{timeOfDay12Hours}:{Mathf.Floor(timeOfDayMinutes)} {EnglishTime}";
    public string GetTime24Hour => $"{timeOfDayHours}:{Mathf.Floor(timeOfDayMinutes)}";

    public int TimeOfDayHours { get => timeOfDayHours; set => timeOfDayHours = value; }
    public int TimeOfDay12Hours { get => timeOfDay12Hours; set => timeOfDay12Hours = value; }
    public int TimeOfSunrise => timeOfSunrise;
    public int TimeOfSunset => timeOfSunset;
    private CultureInfo usInfo;

    public string GetDate()
    {
        if (usInfo == null)
        {
            usInfo = new CultureInfo("en-US", false);
        }
        if (date.Year == 1)
        {
            long ticks = new DateTime(currentYear, currentMonth, currentDay, 0, 0, 0,
            usInfo.Calendar).Ticks;
            date = new DateTime(ticks);
        }

        //Thread.CurrentThread.CurrentCulture = usInfo;
        return date.ToString("d");
    }

    public int CurrentDay
    {
        get => currentDay;
        set
        {
            bool newMonth = (CheckForNewMonth(currentDay));
            currentDay = value;

            date.AddDays(1);

            if (newMonth)
                onMonthChanged.Raise(date.Month);

            onDayChanged.Raise(currentDay);
        }
    }

    private bool CheckForNewMonth(int dayBeforeChange)
    {
        int amountDaysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
        return (amountDaysInMonth == dayBeforeChange);
    }

    public float TimeOfDayMinutes
    {
        get => timeOfDayMinutes;
        set
        {
            if(Mathf.FloorToInt(timeOfDayMinutes) != Mathf.FloorToInt(value))
            {
                onTime24Changed.Raise(GetTime24Hour);
                onTime12Changed.Raise(GetTime12Hour);
            }
            timeOfDayMinutes = value;
        }
    }

    public void Reset()
    {
        timeOfDayHours = 0;
        timeOfDay12Hours = 0;
        currentDay = 1;
        timeOfDayMinutes = 0;
    }
}