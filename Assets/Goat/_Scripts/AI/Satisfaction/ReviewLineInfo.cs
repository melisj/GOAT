using UnityEngine;

[System.Serializable]
public class ReviewLineInfo
{
    [SerializeField] private string line;
    [SerializeField] private int points;
    [SerializeField] private ReviewFactorType factorType;

    public ReviewLineInfo(string line, int points, ReviewFactorType factorType)
    {
        this.line = line;
        this.points = points;
        this.factorType = factorType;
    }

    public string Line => line;
    public int Points => points;
    public ReviewFactorType FactorType => factorType;
}

public enum ReviewFactorType
{
    beauty,
    notFound,
    searchTime
}