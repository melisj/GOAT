using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.AI.Parking
{
    [CreateAssetMenu(fileName = "Parking", menuName = "ScriptableObjects/RuntimeVariables/Parking")]
    public class Parking : ScriptableObject
    {
        /// <summary>
        /// Get the current spot the spawner is busy with
        /// </summary>
        public ParkingSpots.ParkingSpot CurrentSpotSpawing { get; set; }
    }
}
