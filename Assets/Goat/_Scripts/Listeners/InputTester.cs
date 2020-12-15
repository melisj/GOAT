using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Events;
using Goat;

public class InputTester : EventListenerKeyCodeModeEvent
{
    public override void OnEventRaised(KeyCodeMode value)
    {
        KeyCode code = KeyCode.None;
        KeyMode mode = KeyMode.None;

        value.Deconstruct(out code, out mode);

        Debug.Log($"{code}\t{mode}");
    }
}