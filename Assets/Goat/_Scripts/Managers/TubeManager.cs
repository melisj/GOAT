using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Farming
{
    public class TubeManager : MonoBehaviour
    {
        private TubeEnd[] ends;

        private FarmStationFunction[] farms;

        private void InitNetworkData()
        {
            ends = FindObjectsOfType<TubeEnd>();
            farms = FindObjectsOfType<FarmStationFunction>();
        }

        [Button("Connect network test")]
        private void StartNetworkConnection()
        {
            InitNetworkData();
            ConnectNetwork();
        }

        private void ConnectNetwork()
        {

        }
    }
}