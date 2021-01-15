using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Farming;

public class FarmRangeParticle : MonoBehaviour
{
    [SerializeField] private FarmStationFunction farmFunction;
    private ParticleSystem ps;

    public ParticleSystem ParticleSystem { get => ps; set => ps = value; }

    private void Start()
    {
        ps = gameObject.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startSize = 1 + (farmFunction.Settings.Range * 2);
    }
}