﻿using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Goat.Storage
{
    public enum ResourceType
    {
        Geode,
        Gold,
        Oil
    }

    public enum StorageEnviroment
    {
        normal,
        hot,
        cold
    }

    [CreateAssetMenu(fileName = "Resource", menuName = "ScriptableObjects/Buyable/Resource")]
    public class Resource : Buyable
    {
        [SerializeField, Space(10)] private ResourceType resourceType;
        [SerializeField] private StorageEnviroment storageEnviroment;
        [SerializeField] private int starterAmount;
        [SerializeField] private bool available = true;
        //[SerializeField] private float resValue;
        //[SerializeField] private int amount;
        // [SerializeField] private Sprite image;
        //  private int oldAmount = 0;

        //  public float Price => resValue * 1.5f;
        public int StarterAmount => starterAmount;

        public ResourceType ResourceType => resourceType;

        public StorageEnviroment StorageEnviroment => storageEnviroment;

        public bool Available { get => available; set => available = value; }
        // public Sprite Image => image;
        // public float ResValue { get => resValue; set => resValue = value; }
    }
}