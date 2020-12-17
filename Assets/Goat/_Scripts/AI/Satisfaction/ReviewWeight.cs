using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ReviewWeight", menuName = "ScriptableObjects/GlobalVariables/ReviewWeight")]
public class ReviewWeight : ScriptableObject
{
    [SerializeField, Range(0.1f, 100)] private float weight;
    public float Weight { get => weight; set => weight = value; }
}