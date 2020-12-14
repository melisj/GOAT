﻿using Goat.Grid;
using Goat.Pooling;
using Goat.Storage;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityAtoms.BaseAtoms;

namespace Goat.Farming
{
    public class FarmStationFunction : SerializedMonoBehaviour, IPoolObject
    {
        private const float delay = 1f;
        [SerializeField, InlineEditor, AssetList(Path = "/Goat/ScriptableObjects/Farming")] private FarmStation farmStationSettings;
        [SerializeField] private float radius = 1;
        [SerializeField] private GameObject resPackPrefab;
        [SerializeField] private int currentCapacity;
        [SerializeField] private LayerMask floorLayer;
        [SerializeField] private Animator animator;
        [SerializeField] private int debugPathIndex;
        [SerializeField] private List<Path> connectedTubes = new List<Path>();
        [SerializeField] private VoidEvent onGridChange;
        private Dictionary<Vector3, int> offsetToPath = new Dictionary<Vector3, int>();
        private float timer;
        private bool isConnected;
        private ResourceTile resourceTile;
        [SerializeField] private HashSet<GameObject> tubeEnds = new HashSet<GameObject>();
        [SerializeField] private List<ResourcePack> resPacks = new List<ResourcePack>();
        public Dictionary<Vector3, int> OffsetToPath => offsetToPath;

        public int GetPath(Vector3 key)
        {
            int index = 0;
            if (offsetToPath.TryGetValue(key, out index))
            {
                return index;
            }
            return index;
        }

        public int PoolKey { get; set; }
        public ObjectInstance ObjInstance { get; set; }
        public List<Path> ConnectedTubes => connectedTubes;
        public HashSet<GameObject> TubeEnds => tubeEnds;

        public List<ResourcePack> ResPacks => resPacks;

        private void Awake()
        {
            connectedTubes.Add(new Path());
        }

        private void OnEnable()
        {
            onGridChange.Raise();
        }

        private void OnDisable()
        {
            onGridChange.Raise();
        }

        private void Update()
        {
            AddResource();
        }

        public ResourcePack CreateResourcePack(Vector3 pos, GameObject tubeEnd)
        {
            if (!tubeEnds.Add(tubeEnd))
                return null;
            GameObject resPackObj = PoolManager.Instance.GetFromPool(resPackPrefab, pos, Quaternion.identity, null);
            resPackObj.name = "ResourcePack-" + farmStationSettings.ResourceFarm.name.ToString();
            ResourcePack resPack = resPackObj.GetComponent<ResourcePack>();
            resPack.SetupResPack(farmStationSettings.ResourceFarm, 0);
            resPacks.Add(resPack);
            return resPack;
        }

        private void AddResource()
        {
            if (currentCapacity >= farmStationSettings.StorageCapacity || resourceTile.Amount <= 0)
            {
                animator.enabled = false;
                timer = 0;
                return;
            }
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                animator.enabled = true;
                timer = 0;
                currentCapacity += farmStationSettings.AmountPerSecond;
                resourceTile.Amount -= farmStationSettings.AmountPerSecond;

                if (farmStationSettings.FarmDeliverType == FarmDeliverType.AutoContinuously)
                {
                    FillResourcePacks();
                }

                if (farmStationSettings.FarmType == FarmType.OverTimeCost)
                {
                    farmStationSettings.ResourceFarm.Money.Amount -= farmStationSettings.CostPerSecond;
                }
            }
        }

        private void FillResourcePacks()
        {
            if (resPacks.Count <= 0) return;
            float increment = (float)farmStationSettings.AmountPerSecond / (float)resPacks.Count;

            for (int i = 0; i < resPacks.Count; i++)
            {
                ResourcePack resPack = resPacks[i];
                if (!resPack.gameObject.activeInHierarchy)
                {
                    resPacks.RemoveAt(i);
                    continue;
                }

                resPacks[i].Amount += increment;
            }

            currentCapacity -= farmStationSettings.AmountPerSecond;
        }

        public void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            Setup();
            ObjInstance = objectInstance;
            PoolKey = poolKey;
        }

        private void Setup()
        {
            GetResourceTile();
            onGridChange.Raise();
        }

        private void GetResourceTile()
        {
            resourceTile = null;
            Collider[] cols = Physics.OverlapSphere(transform.position, radius, floorLayer);
            if (cols.Length > 0)
            {
                resourceTile = cols[0].gameObject.GetComponent<ResourceTile>();
            }
        }

        public void OnReturnObject()
        {
            gameObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, radius);
            if (connectedTubes[debugPathIndex] == null || connectedTubes[debugPathIndex].Points == null) return;
            Gizmos.color = Color.yellow;
            for (int i = 0; i < connectedTubes[debugPathIndex].Points.Count; i++)
            {
                if (i <= 0)
                {
                    Gizmos.DrawLine(transform.position, connectedTubes[debugPathIndex].Points[i]);
                }
                else if (i + 1 < connectedTubes[debugPathIndex].Points.Count)
                {
                    Gizmos.DrawLine(connectedTubes[debugPathIndex].Points[i], connectedTubes[debugPathIndex].Points[i + 1]);
                }
            }
        }
    }
}