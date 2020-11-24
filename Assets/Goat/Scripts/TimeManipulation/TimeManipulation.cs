using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManipulation : MonoBehaviour
{
    public float scaleOfTime = 1f;
    void Update()
    {          
            Time.timeScale = scaleOfTime;       
    }
    public void Pause() 
    {
        scaleOfTime = 0f;
      
    }
   public void Play()
    {
        scaleOfTime = 01f;
    }
    public void Forward()
    {
        scaleOfTime = 10f;
    }
    public void FastforwardForward()
    {
        scaleOfTime = 15f;
    }
}