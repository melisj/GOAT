using UnityEngine;
using UnityAtoms.BaseAtoms;
using Sirenix.OdinInspector;

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

    [Title("Events")]
    [SerializeField] private StringEvent onTime24Changed;
    [SerializeField] private StringEvent onTime12Changed;
    [SerializeField] private IntEvent onDayChanged;

    public string EnglishTime => englishTime = timeOfDayHours / 12 >= 1 ? PM : AM;
    public string GetTime12Hour => $"{timeOfDay12Hours}:{Mathf.Floor(timeOfDayMinutes)} {englishTime}";
    public string GetTime24Hour => $"{timeOfDayHours}:{Mathf.Floor(timeOfDayMinutes)}";

    public int TimeOfDayHours { get => timeOfDayHours; set => timeOfDayHours = value; }
    public int TimeOfDay12Hours { get => timeOfDay12Hours; set => timeOfDay12Hours = value; }
    public int TimeOfSunrise => timeOfSunrise;
    public int TimeOfSunset => timeOfSunset;

    public int CurrentDay
    {
        get => currentDay;
        set
        {
            currentDay = value;
            onDayChanged.Raise(currentDay);
        }
    }

    public float TimeOfDayMinutes
    {
        get => timeOfDayMinutes;
        set
        {
            onTime24Changed.Raise(GetTime24Hour);
            onTime12Changed.Raise(GetTime12Hour);
            timeOfDayMinutes = value;
        }
    }

    public void Reset()
    {
        timeOfDayHours = 0;
        timeOfDay12Hours = 0;
        currentDay = 0;
        timeOfDayMinutes = 0;
    }
}