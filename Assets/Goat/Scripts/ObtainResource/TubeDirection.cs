using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Farming
{
    public class TubeDirection : MonoBehaviour
    {
        [SerializeField] private FarmStationFunction connectedFarm;
        public FarmStationFunction ConnectedFarm { get => connectedFarm; set => connectedFarm = value; }
    }
}