using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Review
{
    [SerializeField] private int[] reviewFactors;
    public float SatisfactionPoints { get; set; }
    public int[] ReviewFactors { get => reviewFactors; set => reviewFactors = value; }
}