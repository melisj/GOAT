﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Storage;
using Goat.Grid;

public class StarterKit : MonoBehaviour
{
    [SerializeField] private Money money;
    [SerializeField] private Electricity electricity;

    private void Awake()
    {
        ResetValues();
    }

    private void ResetValues()
    {
        Resource[] resources = Resources.LoadAll<Resource>("Resource");
        for (int i = 0; i < resources.Length; i++)
        {
            resources[i].Amount = resources[i].StarterAmount;
        }
        money.Amount = money.StarterAmount;
    }

    private void OnDisable()
    {
        electricity.ClearAll();
    }
}