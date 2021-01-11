using Goat.Grid.UI;
using Goat.Pooling;
using Goat.Storage;
using Sirenix.OdinInspector;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Goat.Grid.Interactions
{
    /// <summary>
    /// Base script for every interactable object in the game
    /// Contains information of the object
    /// </summary>
    public class BaseInteractable : MonoBehaviour, IPoolObject
    {
        [SerializeField] protected InteractablesInfo info;
        [SerializeField] private GridUIInfo gridUIInfo;
        [SerializeField] private Electricity electricityinfo;
        [SerializeField] private bool adjustPositionAgainstWall;
        [SerializeField, ShowIf("adjustPositionAgainstWall")] private AdjustPositionAgainstWall adjustPosition;

        [TextArea, Space(10)]
        [SerializeField] protected string description;

        [Header("Power Settings")]
        [SerializeField] private bool costsPower;
        [SerializeField, ShowIf("costsPower")] private int powerCost;
        [SerializeField, ShowIf("costsPower")] private bool isPowered;

        [SerializeField] private bool producesPower;
        [SerializeField, ShowIf("producesPower")] private int powerProduction;
        [SerializeField, ShowIf("producesPower")] private bool isPowering;

        protected Vector2Int gridPosition;
        protected Vector3 centerPosition;
        private PlaceableInfo placeableInfo;
        protected Collider clickCollider;

        protected UnityEvent UpdateInteractable = new UnityEvent();
        protected EventHandler<bool> PowerChanged;

        [HideInInspector] public Grid grid;
        private TileAnimation tileAnimation;

        public bool IsClickedOn { get; set; }
        public bool UIOpen => gridUIInfo.IsUIActive;
        public bool UIActivated { get; set; }

        public bool IsPowered
        {
            get { return isPowered; }
            set { isPowered = value; if (isPowered != value) PowerChanged?.Invoke(this, value); }
        }

        public bool IsPowering => isPowering;

        public int PowerCost => powerCost;
        public int PowerProduction => powerProduction;

        public string Description => description;
        public string Name => placeableInfo.Placeable.name;
        public Vector2Int GridPosition { get { return gridPosition; } set { gridPosition = value; } }
        public Vector3 CenterPosition { get { return centerPosition; } set { centerPosition = value; } }

        // Pooling
        public int PoolKey { get; set; }

        public ObjectInstance ObjInstance { get; set; }

        protected virtual void Awake()
        {
            clickCollider = GetComponentInChildren<Collider>();
            placeableInfo = GetComponent<PlaceableInfo>();
            UIActivated = false;
        }

        // Get the event when the object has been clicked
        // If clicked then open UI
        protected virtual void IsClicked(Transform clickedObj)
        {
            //  if (clickCollider.transform == clickedObj)
            IsClickedOn = clickCollider.transform == clickedObj;
        }

        public virtual object[] GetArgumentsForUI()
        {
            return null;
        }

        public void OpenUIFully()
        {
            OpenUI();
            InvokeChange();
        }

        // Open the UI for the this
        public virtual void OpenUI()
        {
            //gridUIInfo.CurrentUIElement = UIElement.Interactable;
            //Debug.Log("Is this even called?");
            info.CurrentSelected = this;
        }

        // Hide this UI
        public virtual void CloseUI()
        {
            //gridUIInfo.CurrentUIElement = UIElement.None;
            //Debug.Log("Apparently yes, but it's closed now");

            IsClickedOn = false;
            info.CurrentSelected = null;
        }

        // Update the UI when something has changed
        protected virtual void InvokeChange()
        {
            UpdateInteractable.Invoke();
        }

        #region Pooling

        public virtual void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            ObjInstance = objectInstance;
            PoolKey = poolKey;
            if (!tileAnimation)
                tileAnimation = GetComponent<TileAnimation>();
            tileAnimation.Prepare();
            tileAnimation.Create();
            UpdateInteractable.AddListener(info.UpdateInteractable);
            SetupElectricity();
            if (adjustPosition != null)
                adjustPosition.Setup();

            InteractableManager.InteractableClickEvt += IsClicked;
        }

        public virtual void OnReturnObject()
        {
            tileAnimation.Destroy(() => gameObject.SetActive(false));

            OnDisableElectricity();
            if (adjustPosition != null)
                adjustPosition.ResetPosition();
            InteractableManager.InteractableClickEvt -= IsClicked;
            UpdateInteractable.RemoveAllListeners();
        }

        protected virtual void OnDestroy()
        {
            InteractableManager.InteractableClickEvt -= IsClicked;
            UpdateInteractable.RemoveAllListeners();
        }

        #endregion Pooling

        #region Electricity

        private void SetupElectricity()
        {
            if (costsPower)
                electricityinfo.AddDevice(this);

            if (producesPower && IsPowering)
                electricityinfo.AddGenerator(this);
            //electricityinfo.Capacity += powerProduction;
        }

        private void OnDisableElectricity()
        {
            if (costsPower)
                electricityinfo.RemoveDevice(this);

            if (producesPower && IsPowering)
                electricityinfo.RemoveGenerator(this);

            //electricityinfo.Capacity -= powerProduction;
        }

        #endregion Electricity
    }
}