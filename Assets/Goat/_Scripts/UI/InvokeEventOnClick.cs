using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

public class InvokeEventOnClick : MonoBehaviour
{
    [SerializeField] private BoolEvent onCycleChange;
    [SerializeField] private Button button;
    [SerializeField] private TimeOfDay timeOfDay;

    private void Awake()
    {
        button.onClick.AddListener(ChangeCycle);
    }

    private void ChangeCycle()
    {
        if (timeOfDay.IsDay)
        {
            timeOfDay.TimeOfDayHours = timeOfDay.TimeOfSunset - 1;
            timeOfDay.TimeOfDayMinutes = 59;
        }
        else
        {
            timeOfDay.TimeOfDayHours = timeOfDay.TimeOfSunrise - 1;
            timeOfDay.TimeOfDayMinutes = 59;
        }
    }
}