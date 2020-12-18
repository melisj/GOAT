using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Goat.Storage
{
    public enum StorageEnviroment
    {
        normal,
        hot,
        cold
    }

    [CreateAssetMenu(fileName = "Resource", menuName = "ScriptableObjects/Buyable/Resource")]
    public class Resource : Buyable
    {
        [SerializeField] private StorageEnviroment storageEnviroment;
        [SerializeField] private bool available = true;

        public StorageEnviroment StorageEnviroment => storageEnviroment;

        public bool Available { get => available; set => available = value; }
    }
}