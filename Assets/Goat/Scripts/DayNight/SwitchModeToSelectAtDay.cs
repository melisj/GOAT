using Goat.Events;

namespace Goat
{
    public class SwitchModeToSelectAtDay : EventListenerBool
    {
        public override void OnEventRaised(bool value)
        {
            InputManager.Instance.InputMode = value ? InputMode.Select : InputMode.Select;
        }
    }
}