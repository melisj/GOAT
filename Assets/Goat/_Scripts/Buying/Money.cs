using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Money", menuName = "ScriptableObjects/GlobalVariables/Money")]
public class Money : ScriptableObject
{
    private float oldAmount;
    [SerializeField] private float amount;
    [SerializeField] private int starterAmount;
    public float OldAmount => oldAmount;

    public event EventHandler<float> AmountChanged;

    public int StarterAmount => starterAmount;

    public float Amount
    {
        get => amount;
        set
        {
            oldAmount = amount;
            amount = value;
            if (Application.isPlaying)
                AmountChanged?.Invoke(this, value);
        }
    }
}