using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Events;
using UnityAtoms;
using TMPro;

namespace Goat.UI
{
    public class TimeTillDelivery : EventListenerVoid
    {
        [SerializeField] private TextMeshProUGUI timeTM;
        [SerializeField] private TimeOfDay time;

        public override void OnEventRaised(Void value)
        {
            timeTM.text = time.TimeTillDay;
        }
    }
}