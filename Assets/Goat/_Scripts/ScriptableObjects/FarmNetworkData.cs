using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Farming
{
    // Saves data about the current pipe network in runtime
    // Saves all active farms and exchange points for the pipes
    [CreateAssetMenu(fileName= "FarmNetworkData", menuName= "ScriptableObjects/RuntimeVariables/FarmNetworkData")]
    public class FarmNetworkData : ScriptableObject
    {
        [SerializeField, ReadOnly] private List<FarmStationFunction> farms = new List<FarmStationFunction>();
        [SerializeField, ReadOnly] private List<TubeDirection> pipes = new List<TubeDirection>();

        public List<FarmStationFunction> Farms => farms;
        public List<TubeDirection> Pipes => pipes;

        public void AddFarm(FarmStationFunction farm)
        {
            if(!farms.Contains(farm))
                farms.Add(farm);
        }

        public void AddPipe(TubeDirection tube)
        {
            if (!pipes.Contains(tube))
                pipes.Add(tube);
        }

        public void RemoveFarm(FarmStationFunction farm)
        {
            if (farms.Contains(farm))
                farms.Remove(farm);
        }

        public void RemovePipe(TubeDirection tube)
        {
            if (pipes.Contains(tube))
                pipes.Remove(tube);
        }
    }

}