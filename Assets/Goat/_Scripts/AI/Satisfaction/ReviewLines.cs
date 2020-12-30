using UnityEngine;

[CreateAssetMenu(fileName = "ReviewLines", menuName = "ScriptableObjects/GlobalVariables/ReviewLines")]
public class ReviewLines : ScriptableObject
{
    [SerializeField] private string[] positiveLines;
    [SerializeField] private string[] negativeLines;

    public string GetPositiveLine => positiveLines[Random.Range(0, positiveLines.Length)];
    public string GetNegativeLine => negativeLines[Random.Range(0, negativeLines.Length)];
}