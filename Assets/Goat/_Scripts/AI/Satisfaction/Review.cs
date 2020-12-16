using System;
using System.Collections;
using System.Collections.Generic;

public class Review
{
    private int[] reviewFactors;
    public float SatisfactionPoints { get; set; }
    public int[] ReviewFactors { get => reviewFactors; set => reviewFactors = value; }
}