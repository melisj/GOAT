using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Farming
{
    public class FarmRangePlane : MonoBehaviour
    {
        [SerializeField] private FarmStationFunction farmStation;

        private void Start()
        {
            transform.localScale = Vector3.one * (1 + (farmStation.Settings.Range * 2));
        }
    }
}