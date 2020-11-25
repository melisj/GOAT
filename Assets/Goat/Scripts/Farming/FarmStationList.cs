using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Farming
{
    [CreateAssetMenu(fileName = "FarmStationList", menuName = "ScriptableObjects/FarmStationList")]
    public class FarmStationList : SerializedScriptableObject
    {
        [SerializeField] private List<FarmStationSettings> farmStations;
        public List<FarmStationSettings> FarmStations => farmStations;
    }
}