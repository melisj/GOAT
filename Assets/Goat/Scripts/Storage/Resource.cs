﻿using System;
using UnityEngine;

namespace Goat.Resource
{
    public enum ResourceType
    {
        type1,
        type2,
        type3
    }

    [CreateAssetMenu(fileName = "Resource", menuName = "ScriptableObjects/Resource")]
    public class Resource : ScriptableObject
    {
        [SerializeField] private ResourceType resourceType;
        [SerializeField] private float resValue;
        [SerializeField] private int amount;
        [SerializeField] private Sprite image;
        private int oldAmount = 0;
        public event EventHandler<int> AmountChanged;

        public int OldAmount => oldAmount;
        public Sprite Image => image;
        public float ResValue { get => resValue; set => resValue = value; }
        public int Amount { 
            get => amount; 
            set 
            {
                oldAmount = amount;
                if (amount <= 0 && value <= 0) 
                {
                    amount = 0;
                }
                else
                {
                    AmountChanged.Invoke(this, value);
                    amount = value;
                }
            } 
        }
    }
}