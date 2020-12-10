using Goat.Storage;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Goat.Events;
using UnityAtoms;

namespace Goat.Farming
{
    public class TubeEnd : MonoBehaviour
    {
        [SerializeField] private List<FarmStationFunction> connectedFarms = new List<FarmStationFunction>();
        [SerializeField] private TubeDirection tubeConnection;
        [SerializeField] private float delay;
        public List<FarmStationFunction> ConnectedFarms => connectedFarms;

        private Sequence createResPackSequence;

        private void Clear()
        {
            connectedFarms.Clear();
        }

        private void Awake()
        {
            createResPackSequence = DOTween.Sequence();
            createResPackSequence.SetLoops(-1);
            createResPackSequence.AppendInterval(delay);
            createResPackSequence.AppendCallback(CreateResPacks);
        }

        private void CreateResPacks()
        {
            for (int i = 0; i < connectedFarms.Count; i++)
            {
                connectedFarms[i].CreateResourcePack();
            }
        }
    }
}