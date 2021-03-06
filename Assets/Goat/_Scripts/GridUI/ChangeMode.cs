﻿using Goat;
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
            gridUIInfo.CurrentUIElement = UIElement.Tiles;
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

    private void Update()
    {
        if (AllowedToChange)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                inputMode.InputMode = InputMode.Edit;
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                inputMode.InputMode = InputMode.Destroy;
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                inputMode.InputMode = InputMode.Select;
            }
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