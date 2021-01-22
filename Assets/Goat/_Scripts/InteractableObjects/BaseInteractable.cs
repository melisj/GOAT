using Goat.Farming.Electricity;
using Goat.Grid.UI;
using Goat.Pooling;
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
        [Required, SerializeField, TabGroup("References")] protected InteractablesInfo info;
        [Required, SerializeField, TabGroup("References")] private GridUIInfo gridUIInfo;
        [SerializeField, TabGroup("References"), ShowIf("selectable")] private MeshRenderer outlineRend;
        [SerializeField, TabGroup("Settings")] private bool adjustPositionAgainstWall;
        [SerializeField, TabGroup("Settings")] private bool selectable = true;
        [SerializeField, TabGroup("Settings"), ShowIf("adjustPositionAgainstWall")] private AdjustPositionAgainstWall adjustPosition;

        [SerializeField, TabGroup("Settings")] private bool producesOrConsumesElectricity;
        private ElectricityComponent electricityComponent;

        protected Vector2Int gridPosition;
        protected Vector3 centerPosition;
        private PlaceableInfo placeableInfo;
        private AudioCue audioCue;
        protected Collider clickCollider;

        protected UnityEvent UpdateInteractable = new UnityEvent();

        private Grid grid;
        public Grid Grid { get => grid; set => grid = value; }
        private TileAnimation tileAnimation;
        private bool isClickedOn;

        public bool IsClickedOn
        {
            get => isClickedOn;
            set
            {
                isClickedOn = value;
            }
        }

        public bool UIActivated { get; set; }

        public string Name => placeableInfo.Placeable.name;
        public Vector2Int GridPosition { get { return gridPosition; } set { gridPosition = value; } }
        public Vector3 CenterPosition { get { return centerPosition; } set { centerPosition = value; } }

        // Pooling
        public int PoolKey { get; set; }

        public ObjectInstance ObjInstance { get; set; }

        protected virtual void Awake()
        {
            if (producesOrConsumesElectricity) electricityComponent = GetComponent<ElectricityComponent>();
            clickCollider = GetComponentInChildren<Collider>();
            placeableInfo = GetComponent<PlaceableInfo>();
            UIActivated = false;
        }

        // Get the event when the object has been clicked
        // If clicked then open UI
        protected virtual void IsClicked(Transform clickedObj)
        {
            IsClickedOn = clickCollider.transform == clickedObj;
            ShowOutline();
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
            info.CurrentSelected = this;
        }

        // Hide this UI
        public virtual void CloseUI()
        {
            info.CurrentSelected = null;
            IsClicked(null);
        }

        private void ShowOutline()
        {
            if (selectable)
                outlineRend.enabled = IsClickedOn;
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
            if (!audioCue)
                audioCue = GetComponent<AudioCue>();
            if (audioCue)
                audioCue.PlayAudioCue();
            tileAnimation.Prepare();
            tileAnimation.Create();
            UpdateInteractable.AddListener(info.UpdateInteractable);
            if (adjustPosition != null)
                adjustPosition.Setup();

            if (producesOrConsumesElectricity)
                electricityComponent.PoweredChangedEvt += ElectricityComponent_PowerChangedEvt;
            InteractableManager.InteractableClickEvt += IsClicked;
        }

        private void ElectricityComponent_PowerChangedEvt(bool powered)
        {
        }

        public virtual void OnReturnObject()
        {
            tileAnimation.Destroy(() => gameObject.SetActive(false));
            if (!audioCue)
                audioCue = GetComponent<AudioCue>();
            if (audioCue)
                audioCue.StopAudioCue();
            if (adjustPosition != null)
                adjustPosition.ResetPosition();
            InteractableManager.InteractableClickEvt -= IsClicked;
            UpdateInteractable.RemoveAllListeners();

            if (producesOrConsumesElectricity)
                electricityComponent.PoweredChangedEvt -= ElectricityComponent_PowerChangedEvt;
        }

        protected virtual void OnDestroy()
        {
            InteractableManager.InteractableClickEvt -= IsClicked;
            UpdateInteractable.RemoveAllListeners();
        }

        #endregion Pooling
    }
}