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
using UnityAtoms;
using ReadOnly = Sirenix.OdinInspector.ReadOnlyAttribute;

namespace Goat.Farming
{
    public class FarmStationFunction : SerializedMonoBehaviour, IAtomListener<WithOwner<TubeDirection>>
    {
        [SerializeField] private TubeDirectionEvent onTubeEndReceived;
        [SerializeField] private GameObjectEvent onTubeEndNeeded;
        [SerializeField] private FarmStation farmStationSettings;
        [SerializeField] private FarmNetworkData networkData;
        public FarmStation Settings => farmStationSettings;

        [SerializeField] private GameObject resPackPrefab;
        [SerializeField] private GameObject rangePlane;
        [SerializeField] private LayerMask floorLayer;
        [SerializeField, ReadOnly] private TubeDirection foundTubeEnd;
        [SerializeField, ReadOnly] private TubeEnd tubeEnd;
        public TubeDirection FoundTubeEnd { get => foundTubeEnd; set { foundTubeEnd = value; tubeEnd = null; } }

        public TubeEnd TubeEnd
        {
            get
            {
                if (tubeEnd == null && FoundTubeEnd != null)
                    tubeEnd = FoundTubeEnd.GetComponent<TubeEnd>();
                return tubeEnd;
            }
        }

        public TubeDirection TubeDirection { get; private set; }
        [SerializeField, ReadOnly] private float timer;

        private Queue<ResourceTile> resourceTiles = new Queue<ResourceTile>();
        private ResourceTile currentResourceTile;
        private Inventory inventory;

        [SerializeField] private AudioCue cue;
        [SerializeField] private Animator animator;

        private bool stopped;

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

        public void FindTubeEnd()
        {
            onTubeEndNeeded.Raise(gameObject);
        }

        public void OnEventRaised(WithOwner<TubeDirection> item)
        {
            if (item.Owner == gameObject)
            {
                if (item.Gtype != null)
                {
                    TubeEnd end = item.Gtype.GetComponent<TubeEnd>();
                    if (end)
                        tubeEnd = end;
                }
                else
                {
                    tubeEnd = null;
                }
            }
        }

        private void Awake()
        {
            TubeDirection = GetComponent<TubeDirection>();
            inventory = new Inventory(Settings.StorageCapacity);
        }

        private void OnEnable()
        {
            onTubeEndReceived.RegisterSafe(this);
            networkData.AddFarm(this);
            rangePlane.SetActive(false);
        }

        private void OnDisable()
        {
            onTubeEndReceived.UnregisterSafe(this);
            networkData.RemoveFarm(this);
            rangePlane.SetActive(true);
            cue.StopAudioCue();
        }

        private void OnDestroy()
        {
            networkData.RemoveFarm(this);
        }

        private void Update()
        {
            AddResource();
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

            if (inventory.ItemsInInventory >= farmStationSettings.StorageCapacity || inventory.ItemsInInventory <= 0)
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

                    if (TubeEnd != null)
                        CreateResourcePack(TubeEnd.SpawnPos);
                }

                if (farmStationSettings.FarmType == FarmType.OverTimeCost)
                {
                    farmStationSettings.Money.Amount -= farmStationSettings.CostPerSecond;
                }
            }
        }

        private void CreateResourcePack(Vector3 pos)
        {
            GameObject resPackObj = PoolManager.Instance.GetFromPool(resPackPrefab, pos, Quaternion.identity, null);
            ResourcePack resPack = resPackObj.GetComponent<ResourcePack>();

            Resource resource = inventory.Items.ElementAt(Random.Range(0, inventory.Items.Count)).Key;
            inventory.Remove(resource, 1, out int amountRemoved);

            resPack.SetupResPack(resource);
        }

        public void SetResourceTile(List<Tile> tiles)
        {
            foreach (Tile tile in tiles)
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

        private void OnDrawGizmos()
        {
            
        }
    }
}