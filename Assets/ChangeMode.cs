using Goat;
using Goat.Grid.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMode : MonoBehaviour
{
    public bool AllowedToChange { get; set; }

    private void Awake()
    {
        InputManager.Instance.OnInputEvent += Instance_OnInputEvent;
    }

    private void Instance_OnInputEvent(KeyCode code, InputManager.KeyMode keyMode, InputMode inputMode)
    {
        if (AllowedToChange)
        {
            ChangeModes(code, keyMode, inputMode);
        }
    }

    private void ChangeModes(KeyCode code, InputManager.KeyMode keyMode, InputMode inputMode)
    {
        if (code == KeyCode.C && keyMode == InputManager.KeyMode.Down)
        {
            GridUIManager.Instance.ShowNewUI(GridUIElement.Building);
            InputManager.Instance.InputMode = InputMode.Edit;
        }
        if (code == KeyCode.B && keyMode == InputManager.KeyMode.Down)
        {
            InputManager.Instance.InputMode = InputMode.Destroy;
        }
        if (code == KeyCode.X && keyMode == InputManager.KeyMode.Down)
        {
            InputManager.Instance.InputMode = InputMode.Select;
        }
    }
}