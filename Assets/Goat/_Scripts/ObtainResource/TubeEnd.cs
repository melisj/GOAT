﻿using DG.Tweening;
using Goat.Storage;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Goat.Farming
{
    public class TubeEnd : SerializedMonoBehaviour
    {
        [SerializeField] private HashSet<GameObject> connectedFarms = new HashSet<GameObject>();
        [SerializeField] private TubeDirection tubeConnection;
        [SerializeField] private VoidEvent onGridChange;
        [SerializeField] private float delay;
        [SerializeField] private float radius = 0.1f;
        [SerializeField] private LayerMask resourcePackLayer;
        public HashSet<GameObject> ConnectedFarms => connectedFarms;
        [SerializeField] private List<ResourcePack> resPacks = new List<ResourcePack>();
        [SerializeField] private Vector3 pos;
        [SerializeField] private Collider[] colls;
        private Sequence createResPackSequence;

        private void Clear()
        {
            connectedFarms.Clear();
        }

        //private void Awake()
        //{
        //    createResPackSequence = DOTween.Sequence();
        //    createResPackSequence.SetLoops(-1);
        //    createResPackSequence.AppendInterval(delay);
        //    createResPackSequence.AppendCallback(CreateResPacks);
        //}
        private void OnEnable()
        {
            createResPackSequence = DOTween.Sequence();
            createResPackSequence.SetLoops(-1);
            createResPackSequence.AppendCallback(CreateResPacks);
            createResPackSequence.AppendInterval(delay);
        }

        private void CreateResPacks()
        {
            onGridChange.Raise();
            if (!tubeConnection.HasConnection())
            {
                Clear();
                return;
            }

            pos = tubeConnection.CorrectPosWithRotation(tubeConnection.Path.Points[1]);
            colls = CheckForResPacks(pos);
            RemovePickedResPacks(colls);
            if (colls.Length >= connectedFarms.Count) return;

            var enumerator = connectedFarms.GetEnumerator();
            while (enumerator.MoveNext())
            {
                FarmStationFunction farmStation = enumerator.Current.GetComponent<FarmStationFunction>();
                ResourcePack resPack = farmStation.CreateResourcePack(pos, gameObject);
                if (resPack)
                {
                    resPacks.Add(resPack);
                }
            }
        }

        private void RemovePickedResPacks(Collider[] colliders)
        {
            var enumerator = connectedFarms.GetEnumerator();
            while (enumerator.MoveNext())
            {
                for (int i = 0; i < resPacks.Count; i++)
                {
                    if (!Array.Exists(colliders, element => element == resPacks[i].gameObject.GetComponent<Collider>()))
                    {
                        FarmStationFunction farmStation = enumerator.Current.GetComponent<FarmStationFunction>();
                        farmStation.TubeEnds.Remove(gameObject);
                        farmStation.ResPacks.Remove(resPacks[i]);
                        resPacks.RemoveAt(i);
                    }
                }
            }
        }

        private void OnDisable()
        {
            createResPackSequence.Kill();
            pos = tubeConnection.CorrectPosWithRotation(tubeConnection.Path.Points[1]);

            var enumerator = connectedFarms.GetEnumerator();
            while (enumerator.MoveNext())
            {
                for (int i = 0; i < resPacks.Count; i++)
                {
                    FarmStationFunction farmStation = enumerator.Current.GetComponent<FarmStationFunction>();
                    farmStation.TubeEnds.Remove(gameObject);
                    farmStation.ResPacks.Remove(resPacks[i]);
                }
            }

            resPacks.Clear();
        }

        private Collider[] CheckForResPacks(Vector3 pos)
        {
            return Physics.OverlapSphere(pos, radius, resourcePackLayer);
        }

        private void OnDrawGizmos()
        {
            pos = tubeConnection.CorrectPosWithRotation(tubeConnection.Path.Points[1]);
            pos.y = 0;
            Gizmos.DrawWireSphere(pos, radius);
        }
    }
}