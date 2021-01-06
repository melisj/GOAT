using UnityEngine;

[CreateAssetMenu(fileName = "ReviewData", menuName = "ScriptableObjects/ReviewData")]
public class ReviewData : ScriptableObject
{
    [SerializeField, Range(0.1f, 100)] private float weight;
    [SerializeField] private string[] positiveLines;
    [SerializeField] private string[] negativeLines;

    public string GetPositiveLine => positiveLines[Random.Range(0, positiveLines.Length)];
    public string GetNegativeLine => negativeLines[Random.Range(0, negativeLines.Length)];
    public float Weight { get => weight; set => weight = value; }
}