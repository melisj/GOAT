using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayNightCycle : MonoBehaviour
{
    private const string PM = "PM";
    private const string AM = "AM";

    private Light mainLight;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dayText;

    [SerializeField]
    private Color dayTimeColor = new Color(1, 1, 1);
    [SerializeField]
    private Color nightTimeColor = new Color(0, 0, 0);
    [SerializeField] private Image dayNightIcon;
    [SerializeField] private Sprite sun, moon;
    private Color currentTimeColor;
    private Color targetTimeColor;

    //transition timer for lerping the time of day
    private float transitionTimer;
    public bool isDay;
    private string englishTime;
    //current hours and day
    private float timeOfDayMinutes;
    private int timeOfDayHours;
    private int daysIn;

    //which hour of day the sun rises and sets
    private int timeOfSunrise = 8;
    private int timeOfSunset = 20;

    //the regular speed of the day + clock. not to be confused with time manipulation
    private int timeSpeed = 3;

    private void Start()
    {
        mainLight = this.gameObject.GetComponent<Light>();
        //day starts at night
        setTimeNight();
        //TimeOfDayHours = zet hier een tijd om de dag te beginnen
        transitionTimer = 1f;
        mainLight.color = targetTimeColor;
    }

    private void Update()
    {
        UpdateClock();

        if (transitionTimer < 1)
        {
            transitionTimer += Time.deltaTime;
            mainLight.color = Color.Lerp(currentTimeColor, targetTimeColor, transitionTimer);
        }
    }

    private void UpdateClock()
    {
        transitionTimer += Time.unscaledDeltaTime;
        timeOfDayMinutes += Time.deltaTime * timeSpeed;

        timeText.text = $"{timeOfDayHours}:{Mathf.Floor(timeOfDayMinutes)} {englishTime}";
        dayText.text = $"Day {daysIn}";
        if (timeOfDayMinutes > 60)
        {
            timeOfDayMinutes = 0;
            timeOfDayHours += 1;

            //check if its morning, nighttime or midnight
            if (timeOfDayHours == timeOfSunrise)
            {
                setTimeDay();
            }
            else if (timeOfDayHours == timeOfSunset)
            {
                setTimeNight();
            }
            else if (timeOfDayHours == 24)
            {
                daysIn++;
                timeOfDayHours = 0;
            }
        }
    }

    private void setTimeDay()
    {
        //from nighttime to daytime
        currentTimeColor = nightTimeColor;
        targetTimeColor = dayTimeColor;
        dayNightIcon.sprite = sun;
        englishTime = PM;
        isDay = true;
        transitionTimer = 0f;
    }

    private void setTimeNight()
    {
        //from daytime to nighttime
        currentTimeColor = dayTimeColor;
        targetTimeColor = nightTimeColor;
        dayNightIcon.sprite = moon;
        englishTime = AM;
        isDay = false;
        transitionTimer = 0f;
    }
}