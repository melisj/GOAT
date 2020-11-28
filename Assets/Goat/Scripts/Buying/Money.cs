using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Money", menuName = "ScriptableObjects/Money")]
public class Money : ScriptableObject
{
    private float oldAmount;
    [SerializeField] private float amount;

    public float OldAmount => oldAmount;

    public event EventHandler<float> AmountChanged;

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