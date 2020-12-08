using System.Collections.Generic;
using UnityEngine;

namespace Goat.Farming
{
    [System.Serializable]
    public class Path
    {
        [SerializeField] private List<Vector3> points = new List<Vector3>();

        public List<Vector3> Points { get => points; set => points = value; }

        // public List<Vector3> Points { get => points; set => points = value; }
    }
}