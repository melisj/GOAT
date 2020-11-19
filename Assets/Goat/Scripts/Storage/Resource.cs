using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Goat.Resource
{
    public enum ResourceType
    {
        Orichalcum,
        SilverStone,
        Nanite
    }

    [CreateAssetMenu(fileName = "Resource", menuName = "ScriptableObjects/Resource")]
    public class Resource : SerializedScriptableObject
    {
        [SerializeField] private ResourceType resourceType;
        [SerializeField] private float resValue;
        [SerializeField] private int amount;
        [SerializeField] private Sprite image;
        private int oldAmount = 0;
        public event EventHandler<int> AmountChanged;

        public ResourceType ResourceType => resourceType;
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
                    amount = value;
                }
                AmountChanged.Invoke(this, value);
            } 
        }
    }
}