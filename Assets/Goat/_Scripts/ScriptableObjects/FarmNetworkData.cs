using System.Collections.Generic;
using UnityEngine;

namespace Goat.Farming
{
    [CreateAssetMenu(fileName= "FarmNetworkData", menuName= "ScriptableObjects/RuntimeVariables/FarmNetworkData")]
    public class FarmNetworkData : ScriptableObject
    {
        [SerializeField] private List<FarmStationFunction> farms = new List<FarmStationFunction>();
        [SerializeField] private List<TubeDirection> pipes = new List<TubeDirection>();

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