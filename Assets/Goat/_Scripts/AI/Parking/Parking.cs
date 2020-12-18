using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.AI.Parking
{
    [CreateAssetMenu(fileName = "Parking", menuName = "ScriptableObjects/GlobalVariables/Parking")]
    public class Parking : ScriptableObject
    {
        public GameObject shipPrefab;
        public GameObject warpEffectPrefab;
    }
}
