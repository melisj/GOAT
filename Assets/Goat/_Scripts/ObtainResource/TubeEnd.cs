using DG.Tweening;
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
        [SerializeField] private float delay;
        [SerializeField] private float radius = 0.1f;
        [SerializeField] private LayerMask resourcePackLayer;
        public HashSet<GameObject> ConnectedFarms => connectedFarms;
        //[SerializeField] private List<ResourcePack> resPacks = new List<ResourcePack>();
        [SerializeField] private Vector3 pos;
        [SerializeField] private Collider[] colls;
        public Vector3 SpawnPos => tubeConnection.CorrectPosWithRotation(pos);
        /*  public TubeEndInfo Info
          {
              get
              {
                  float[] amount = new float[resPacks.Count];
                  int[] resource = new int[resPacks.Count];
                  for (int i = 0; i < resPacks.Count; i++)
                  {
                      amount[i] = 1;
                      resource[i] = resPacks[i].Resource.ID;
                  }
                  return new TubeEndInfo(resource, amount, transform.position, (int)transform.rotation.eulerAngles.y / 90);
              }
          }*/

        //private void Clear()
        //{
        //    connectedFarms.Clear();
        //}

        //private void Awake()
        //{
        //    createResPackSequence = DOTween.Sequence();
        //    createResPackSequence.SetLoops(-1);
        //    createResPackSequence.AppendInterval(delay);
        //    createResPackSequence.AppendCallback(CreateResPacks);
        //}
        //private void OnEnable()
        //{
        //    calls = 0;
        //    createResPackSequence = DOTween.Sequence();
        //    createResPackSequence.SetLoops(-1);
        //    createResPackSequence.AppendInterval(delay);
        //    createResPackSequence.AppendCallback(CreateResPacks);
        //}

        //private void CreateResPacks()
        //{
        //    if (!tubeConnection.HasConnection() || ConnectedFarms.Count <= 0)
        //    {
        //        Clear();
        //        return;
        //    }

        //    //pos = tubeConnection.CorrectPosWithRotation(tubeConnection.Path.Points[1]);
        //}

        //private void OnDisable()
        //{
        //    createResPackSequence.Kill();
        //}
        /*
                public void CreateResPacks(List<Resource> resources, float[] amount)
                {
                    pos = tubeConnection.CorrectPosWithRotation(tubeConnection.Path.Points[1]);

                    var enumerator = connectedFarms.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        FarmStationFunction farmStation = enumerator.Current.GetComponent<FarmStationFunction>();
                        for (int i = 0; i < farmStation.Settings.ResourceFarms.Length; i++)
                        {
                            Resource res = farmStation.Settings.ResourceFarms[i];
                            if (resources.Contains(res))
                            {
                                int resourceIndex = resources.IndexOf(res);
                                ResourcePack resPack = farmStation.CreateResourcePack(pos, gameObject, (int)amount[resourceIndex]);
                                if (resPack)
                                {
                                    resPacks.Add(resPack);
                                }
                            }
                        }
                    }
                }*/

        /*private void RemovePickedResPacks(Collider[] colliders)
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
                        resPacks.RemoveAt(i);
                    }
                }
            }
        }*/

        /*private void OnDisable()
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
                }
            }

            resPacks.Clear();
        }*/

        /*private Collider[] CheckForResPacks(Vector3 pos)
        {
            return Physics.OverlapSphere(pos, radius, resourcePackLayer);
        }*/

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(SpawnPos, radius);
        }
    }
}