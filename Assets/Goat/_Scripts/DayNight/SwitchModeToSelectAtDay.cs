using Goat.Events;
using UnityEngine;

namespace Goat
{
    public class SwitchModeToSelectAtDay : EventListenerBool
    {
        [SerializeField] private InputModeVariable currentMode;

        public override void OnEventRaised(bool value)
        {
            currentMode.InputMode = value ? InputMode.Select : InputMode.Select;
        }
    }
}