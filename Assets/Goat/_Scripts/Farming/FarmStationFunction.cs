using Goat.Grid;
using Goat.Pooling;
using Goat.Storage;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityAtoms.BaseAtoms;
using System;

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
        [SerializeField] private GameObjectEvent onGridChange;
        private Dictionary<Vector3, int> offsetToPath = new Dictionary<Vector3, int>();
        private float timer;
        private bool isConnected;
        private ResourceTile resourceTile;
        private TileAnimation tileAnimation;
        [SerializeField] private HashSet<GameObject> tubeEnds = new HashSet<GameObject>();
        [SerializeField] private List<ResourcePack> resPacks = new List<ResourcePack>();
        public Dictionary<Vector3, int> OffsetToPath => offsetToPath;
        [SerializeField] private AudioCue cue;
        private bool stopped;
        public FarmStation Settings => farmStationSettings;

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

        private bool Stopped
        {
            get => stopped;
            set
            {
                if (value != stopped)
                {
                    if (value)
                        cue.StopAudioCue();
                    else
                        cue.PlayAudioCue();
                }
                stopped = value;
            }
        }

        private void Awake()
        {
            connectedTubes.Add(new Path());
        }

        private void OnEnable()
        {
            onGridChange.Raise(null);
        }

        private void OnDisable()
        {
            onGridChange.Raise(null);
        }

        private void Update()
        {
            AddResource();
        }

        public ResourcePack CreateResourcePack(Vector3 pos, GameObject tubeEnd, int amount = 0)
        {
            if (!tubeEnds.Add(tubeEnd))
                return null;
            GameObject resPackObj = PoolManager.Instance.GetFromPool(resPackPrefab, GetGroundPositionAt(pos), Quaternion.identity, null);
            GetResourceTile();
            resPackObj.name = "ResourcePack-" + resourceTile.Data.Resource.name.ToString();
            ResourcePack resPack = resPackObj.GetComponent<ResourcePack>();
            resPack.SetupResPack(resourceTile.Data.Resource, amount);
            resPacks.Add(resPack);
            return resPack;
        }

        private Vector3 GetGroundPositionAt(Vector3 pos)
        {
            RaycastHit hit = new RaycastHit();
            Physics.Raycast(pos, Vector3.down, out hit, floorLayer);
            return hit.point;
        }

        private void AddResource()
        {
            if (resourceTile == null) GetResourceTile();

            if (currentCapacity >= farmStationSettings.StorageCapacity || (resourceTile != null && resourceTile.Amount <= 0))
            {
                animator.enabled = false;
                Stopped = true;
                timer = 0;
                return;
            }
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                animator.enabled = true;
                Stopped = false;

                timer = 0;
                currentCapacity += farmStationSettings.AmountPerSecond;
                resourceTile.Amount -= farmStationSettings.AmountPerSecond;

                if (farmStationSettings.FarmDeliverType == FarmDeliverType.AutoContinuously)
                {
                    FillResourcePacks();
                }

                if (farmStationSettings.FarmType == FarmType.OverTimeCost)
                {
                    farmStationSettings.Money.Amount -= farmStationSettings.CostPerSecond;
                }
            }
        }

        private void FillResourcePacks()
        {
            if (resPacks.Count <= 0)
            {
                onGridChange.Raise(null);
                return;
            }
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
            if (!tileAnimation)
                tileAnimation = GetComponent<TileAnimation>();
            tileAnimation.Prepare();
            Setup();
            ObjInstance = objectInstance;
            PoolKey = poolKey;
            tileAnimation.Create();
        }

        private void Setup()
        {
            GetResourceTile();
            onGridChange.Raise(gameObject);
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
            cue.StopAudioCue();
            tileAnimation.Destroy(() => gameObject.SetActive(false));
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, radius);
            if (connectedTubes.Count <= debugPathIndex) return;
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