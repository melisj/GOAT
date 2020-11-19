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
    private bool isDay;

    //current hours and day
    private float TimeOfDayMinutes;
    private int TimeOfDayHours;

    //which hour of day the sun rises and sets
    private int TimeOfSunrise = 8;
    private int TimeOfSunset = 20;
    

    //the regular speed of the day + clock. not to be confused with time manipulation
    private int timeSpeed = 90;


    void Start()
    {
        mainLight = this.gameObject.GetComponent<Light>();
        setTimeNight();
        transitionTimer = 1f;
    }

    void Update()
    {
        transitionTimer += Time.deltaTime;

        UpdateClock();
        
        if (transitionTimer < 1)
            mainLight.color = Color.Lerp(currentTimeColor, targetTimeColor, transitionTimer);
        
    }

    private void UpdateClock()
    {
        transitionTimer += Time.deltaTime;
        TimeOfDayMinutes += Time.deltaTime * timeSpeed;

        timeText.text = $"{TimeOfDayHours}:{Mathf.Floor(TimeOfDayMinutes)}";

        if (TimeOfDayMinutes > 60)
        {
            TimeOfDayMinutes = 0;
            TimeOfDayHours += 1;

            //check if its morning, nighttime or midnight
            if (TimeOfDayHours == TimeOfSunrise)
            {
                setTimeDay();
            } else if (TimeOfDayHours == TimeOfSunset)
            {
                setTimeNight();
            } else if (TimeOfDayHours == 24)
            {
                TimeOfDayHours = 0;
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
