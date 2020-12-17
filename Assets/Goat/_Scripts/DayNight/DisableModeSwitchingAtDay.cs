using UnityEngine;
using Goat.Events;

namespace Goat
{
    public class DisableModeSwitchingAtDay : EventListenerBool
    {
        [SerializeField] private ChangeMode changeMode;

        public override void OnEventRaised(bool value)
        {
            changeMode.AllowedToChange = !value;
        }
    }
}