using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ReviewWeight", menuName = "ScriptableObjects/GlobalVariables/ReviewWeight")]
public class ReviewWeight : ScriptableObject
{
    [SerializeField, ProgressBar(0f, 1f)] private float weight;
    public float Weight { get => weight; set => weight = value; }
}