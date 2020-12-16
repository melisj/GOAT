using Goat.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSatisfaction : EventListenerReview
{
    [SerializeField] private List<Review> reviews = new List<Review>();
    [SerializeField] private SatisfactionLevel satisfactionLevel;

    public override void OnEventRaised(Review value)
    {
        reviews.Add(value);
        satisfactionLevel.Satisfaction += (int)value.SatisfactionPoints;
    }
}