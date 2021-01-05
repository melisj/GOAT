using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Review
{
    [SerializeField] private ReviewLineInfo[] reviewFactors;
    public string CustomerName { get; set; }
    public float SatisfactionPoints { get; set; }
    public ReviewLineInfo[] ReviewFactors { get => reviewFactors; set => reviewFactors = value; }
}