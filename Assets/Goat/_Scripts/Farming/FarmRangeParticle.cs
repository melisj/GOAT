using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Farming;

    public class FarmRangeParticle : MonoBehaviour
    {
        ParticleSystem ps;

        void Start()
        {
            ps = gameObject.GetComponent<ParticleSystem>();
            var main = ps.main;
            main.startSize = 1 + (GetComponentInParent<FarmStation>().Range * 2);
        }
    }
