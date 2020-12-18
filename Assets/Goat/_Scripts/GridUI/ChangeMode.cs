using Goat;
using Goat.Events;
using Goat.Grid.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Goat.InputManager;

public class ChangeMode : EventListenerKeyCodeModeEvent
{
    [SerializeField] private GridUIInfo gridUIInfo;
    [SerializeField] private InputModeVariable inputMode;
    public bool AllowedToChange { get; set; }

    private void ChangeModes(KeyCode code, KeyMode keyMode)
    {
        if (code == KeyCode.C && keyMode == KeyMode.Down)
        {
            gridUIInfo.CurrentUIElement = GridUIElement.Building;
            inputMode.InputMode = InputMode.Edit;
        }
        if (code == KeyCode.B && keyMode == KeyMode.Down)
        {
            inputMode.InputMode = InputMode.Destroy;
        }
        if (code == KeyCode.X && keyMode == KeyMode.Down)
        {
            inputMode.InputMode = InputMode.Select;
        }
    }

    public override void OnEventRaised(KeyCodeMode value)
    {
        if (AllowedToChange)
        {
            KeyCode code = KeyCode.None;
            KeyMode mode = KeyMode.None;

            value.Deconstruct(out code, out mode);

            ChangeModes(code, mode);
        }
    }
}