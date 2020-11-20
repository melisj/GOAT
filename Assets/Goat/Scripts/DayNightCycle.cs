using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    private Light mainLight;
    public Text timeText;

    [SerializeField]
    private Color dayTimeColor = new Color(1,1,1);
    [SerializeField]
    private Color nightTimeColor = new Color(0,0,0);

    private Color currentTimeColor;
    private Color targetTimeColor;

    //transition timer for lerping the time of day
    private float transitionTimer;
    public bool isDay;

    //current hours and day
    private float timeOfDayMinutes;
    private int timeOfDayHours;

    //which hour of day the sun rises and sets
    private int timeOfSunrise = 8;
    private int timeOfSunset = 20;
    

    //the regular speed of the day + clock. not to be confused with time manipulation
    private int timeSpeed = 3;


    void Start()
    {
        mainLight = this.gameObject.GetComponent<Light>();
        //day starts at night
        setTimeNight();
        //TimeOfDayHours = zet hier een tijd om de dag te beginnen
        transitionTimer = 1f;
        mainLight.color = targetTimeColor;
    }

    void Update()
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

        timeText.text = $"{timeOfDayHours}:{Mathf.Floor(timeOfDayMinutes)}";

        if (timeOfDayMinutes > 60)
        {
            timeOfDayMinutes = 0;
            timeOfDayHours += 1;

            //check if its morning, nighttime or midnight
            if (timeOfDayHours == timeOfSunrise)
            {
                setTimeDay();
            } else if (timeOfDayHours == timeOfSunset)
            {
                setTimeNight();
            } else if (timeOfDayHours == 24)
            {
                timeOfDayHours = 0;
            }
        }
    }

    private void setTimeDay()
    {
        //from nighttime to daytime
        currentTimeColor = nightTimeColor;
        targetTimeColor = dayTimeColor;

        isDay = true;
        transitionTimer = 0f;
    }

    private void setTimeNight()
    {
        //from daytime to nighttime
        currentTimeColor = dayTimeColor;
        targetTimeColor = nightTimeColor;

        isDay = false;
        transitionTimer = 0f;
    }
}
