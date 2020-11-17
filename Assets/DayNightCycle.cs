using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    private Light mainLight;

    [SerializeField]
    private Color dayTimeColor = new Color(1,1,1);
    [SerializeField]
    private Color nightTimeColor = new Color(0,0,0);

    private Color currentTimeColor;
    private Color targetTimeColor;

    private bool isDay;
    private float TransitionTimer;

    void Start()
    {
        mainLight = this.gameObject.GetComponent<Light>();

        setTimeDay();
    }

    void Update()
    {
        TransitionTimer += Time.deltaTime;
        mainLight.color = Color.Lerp(currentTimeColor, targetTimeColor, TransitionTimer);


        if (Input.GetButtonDown("Fire1"))
        {
            if (isDay)
            {
                setTimeNight();
            }
            else
            {
                setTimeDay();
            }
            
        }
    }

    public void setTimeDay()
    {
        currentTimeColor = dayTimeColor;
        targetTimeColor = nightTimeColor;

        isDay = true;
        TransitionTimer = 0f;
    }

    public void setTimeNight()
    {
        currentTimeColor = nightTimeColor;
        targetTimeColor = dayTimeColor;

        isDay = false;
        TransitionTimer = 0f;
    }
}
