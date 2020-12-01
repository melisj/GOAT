using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManipulation : MonoBehaviour
{
    public float scaleOfTime = 1f;
    public float pausetime = 0f;
    public float playTime = 1f;    
    public float forwardTime = 10f;
    public float fastForwardTime = 15f;

    void Update()
    {          
            Time.timeScale = scaleOfTime;       
    }
    public void Pause() 
    {
        scaleOfTime = pausetime ;
      
    }
   public void Play()
    {
        scaleOfTime = playTime;
    }
    public void Forward()
    {
        scaleOfTime = forwardTime;
    }
    public void FastForward()
    {
        scaleOfTime = fastForwardTime;
    }
}