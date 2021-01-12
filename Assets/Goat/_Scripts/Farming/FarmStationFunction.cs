using Goat.Grid;
using Goat.Pooling;
using Goat.Storage;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityAtoms.BaseAtoms;
using System;
using Random = UnityEngine.Random;

namespace Goat.Farming
{
    public class FarmStationFunction : SerializedMonoBehaviour, IPoolObject
    {
        private const float delay = 1f;
        [SerializeField, InlineEditor, AssetList(Path = "/Goat/ScriptableObjects/Farming")] private FarmStation farmStationSettings;
        [SerializeField] private float radius = 1;
        [SerializeField] private GameObject resPackPrefab;
        [SerializeField] private LayerMask floorLayer;
        [SerializeField] private Animator animator;
        [SerializeField] private int debugPathIndex;
        [SerializeField] private List<Path> connectedTubes = new List<Path>();
        [SerializeField] private GameObjectEvent onGridChange;
        private Dictionary<Vector3, int> offsetToPath = new Dictionary<Vector3, int>();
        private float timer;
        private bool isConnected;
        private Queue<ResourceTile> resourceTiles= new Queue<ResourceTile>();
        private Inventory inventory;
        private ResourceTile currentResourceTile;
        private TileAnimation tileAnimation;
        private HashSet<Vector3> tubeEnds = new HashSet<Vector3>();
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
        public HashSet<Vector3> TubeEnds => tubeEnds;

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
            inventory = new Inventory(Settings.StorageCapacity);
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

        public void AddTubeEnd(Vector3 pos)
        {
            if (!tubeEnds.Add(pos))
                return;
        }

        private Vector3 GetGroundPositionAt(Vector3 pos)
        {
            RaycastHit hit = new RaycastHit();
            Physics.Raycast(pos, Vector3.down, out hit, floorLayer);
            return hit.point;
        }

        private void AddResource()
        {
            if (currentResourceTile && !currentResourceTile.gameObject.activeInHierarchy) currentResourceTile = null;  
            if (currentResourceTile == null && resourceTiles.Count != 0) currentResourceTile = resourceTiles.Dequeue();

            if (inventory.ItemsInInventory >= farmStationSettings.StorageCapacity)
            {
                animator.enabled = false;
                Stopped = true;
                timer = 0;
                return;
            }
            timer += Time.deltaTime;
            if (timer >= farmStationSettings.FarmDelay)
            {
                animator.enabled = true;
                Stopped = false;

                timer = 0;

                if (currentResourceTile != null) 
                { 
                    inventory.Add(currentResourceTile.Data.Resource, 1, out int amountStored);
                    currentResourceTile.Amount -= amountStored;
                }

                if (farmStationSettings.FarmDeliverType == FarmDeliverType.AutoContinuously && inventory.ItemsInInventory >= 1)
                {
                    //FillResourcePacks();
                    
                    if(tubeEnds != null && tubeEnds.Count > 0)
                        CreateResourcePack(tubeEnds.ElementAt(UnityEngine.Random.Range(0,tubeEnds.Count)));
                } 

                if (farmStationSettings.FarmType == FarmType.OverTimeCost)
                {
                    farmStationSettings.Money.Amount -= farmStationSettings.CostPerSecond;
                }
            }
        }

        private void CreateResourcePack(Vector3 pos) 
        {
            GameObject resPackObj = PoolManager.Instance.GetFromPool(resPackPrefab, GetGroundPositionAt(pos), Quaternion.identity, null);
            ResourcePack resPack = resPackObj.GetComponent<ResourcePack>();

            Resource resource = inventory.Items.ElementAt(Random.Range(0, inventory.Items.Count)).Key;
            inventory.Remove(resource, 1, out int amountRemoved);

            resPack.SetupResPack(resource);
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
            onGridChange.Raise(gameObject);
        }

        public void SetResourceTile(List<Tile> tiles)
        {
            foreach(Tile tile in tiles)
            {
                if (tile.FloorObj)
                {
                    ResourceTile resourceTile = tile.FloorObj.GetComponent<ResourceTile>();
                    if (resourceTile)
                    {
                        if (farmStationSettings.ResourceFarms.Contains(resourceTile.Data.Resource))
                            resourceTiles.Enqueue(resourceTile);
                    }
                }
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