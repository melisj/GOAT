using Goat.CameraControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ResoucePackMover : MovementSystem
{
    [SerializeField] private Vector3[] tubePoints;
    [SerializeField] private float durationPerTube;

    public void Setup(Vector3[] wayPoints)
    {
        tubePoints = wayPoints;
        transform.DOPath(tubePoints, durationPerTube * tubePoints.Length);
    }
}