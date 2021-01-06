using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class ReviewWeight
{
    [SerializeField, Range(0.1f, 100)] private float weight;
    public float Weight { get => weight; set => weight = value; }
}