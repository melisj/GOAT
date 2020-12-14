using Goat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using UnityAtoms;

[EditorIcon("atom-icon-lush")]
[CreateAssetMenu(menuName = "Unity Atoms/Variables/InputMode", fileName = "InputModeVariable")]
public class InputModeVariable : ScriptableObject
{
    [SerializeField] private InputModeEvent onInputModeChanged;
    [SerializeField] private InputMode inputMode;

    public InputMode InputMode
    {
        get => inputMode;
        set
        {
            onInputModeChanged?.Raise(value);
            inputMode = value;
        }
    }
}