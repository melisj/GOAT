using Goat.Grid.UI;
using Goat.Pooling;
using Goat.Storage;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;

namespace Goat.Grid.Interactions
{
    /// <summary>
    /// This attribute is for tagging value that should be printed out on the informations tab
    /// This attribute can also be used to give a custom name
    /// [TODO] can also flag the way a attribute should be displayed (eg. just numbers or health bar type display or something else)
    /// </summary>
    public class InteractableAttribute : Attribute
    {
        public string customName;

        public InteractableAttribute(string customName = "")
        {
            this.customName = customName;
        }
    }

    /// <summary>
    /// Base script for every interactable object in the game
    /// Contains information of the object
    /// </summary>
    public class BaseInteractable : MonoBehaviour, IPoolObject
    {
        [SerializeField] protected InteractablesInfo info;
        [SerializeField] private GridUIInfo gridUIInfo;
        [SerializeField] private Electricity electricityinfo;

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

        protected Collider clickCollider;

        protected UnityEvent InformationChanged = new UnityEvent();
        protected EventHandler<bool> PowerChanged;

        [HideInInspector] public Grid grid;

        public bool IsClickedOn { get; set; }
        [InteractableAttribute("Powered")] public bool IsPowered { 
            get { return isPowered; } 
            set { isPowered = value; PowerChanged?.Invoke(this, value); } 
        }
        [InteractableAttribute("Powering")] public bool IsPowering => isPowering;

        [InteractableAttribute("Power Cost")] public int PowerCost => powerCost;
        [InteractableAttribute("Power Production")] public int PowerProduction => powerProduction;

        public string Description => description;
        public string Name => name;
        public Vector2Int GridPosition { get { return gridPosition; } set { gridPosition = value; } }
        public Vector3 CenterPosition { get { return centerPosition; } set { centerPosition = value; } }


        // Pooling
        public int PoolKey { get; set; }
        public ObjectInstance ObjInstance { get; set; }


        protected virtual void Awake()
        {
            clickCollider = GetComponentInChildren<Collider>();
        }

        // Get the event when the object has been clicked
        // If clicked then open UI
        protected virtual void IsClicked(Transform clickedObj)
        {
            if(clickCollider.transform == clickedObj)
                IsClickedOn = clickCollider.transform == clickedObj;
        }

        public virtual object[] GetArgumentsForUI() { return null; }

        public void OpenUIFully()
        {
            OpenUI();
            InvokeChange();
        }

        // Open the UI for the this
        public virtual void OpenUI()
        {
            gridUIInfo.CurrentUIElement = GridUIElement.Interactable;
        }

        // Hide this UI
        public virtual void CloseUI()
        {
            gridUIInfo.CurrentUIElement = GridUIElement.None;
            IsClickedOn = false;
        }

        // Update the UI when something has changed
        protected virtual void InvokeChange()
        {
            info.CurrentSelected = this;
            InformationChanged.Invoke();
        }

        // Print out all the variables tagged with "InteractableInfo"
        public virtual string PrintObject(object obj)
        {
            string infoList = "";

            PropertyInfo[] fields = obj.GetType().GetProperties();
            foreach (PropertyInfo field in fields)
            {
                InteractableAttribute meta = (InteractableAttribute)field.GetCustomAttribute(typeof(InteractableAttribute), true);
                if (meta != null)
                {
                    string variableName = meta.customName != "" ? meta.customName : field.Name;
                    infoList += string.Format("{0} - {1}\n", variableName, field.GetValue(obj).ToString());
                }
            }

            return infoList;
        }

        #region Pooling

        public virtual void OnGetObject(ObjectInstance objectInstance, int poolKey) {
            ObjInstance = objectInstance;
            PoolKey = poolKey;

            SetupElectricity();

            InteractableManager.InteractableClickEvt += IsClicked;
        }

        public virtual void OnReturnObject() {
            gameObject.transform.position = new Vector3(-1000, 0);
            gameObject.SetActive(false);

            OnDisableElectricity();

            InteractableManager.InteractableClickEvt -= IsClicked;
            InformationChanged.RemoveAllListeners();
        }

        #endregion

        #region Electricity 

        private void SetupElectricity()
        {
            if (costsPower)
                IsPowered = electricityinfo.AddElectricityConsumption(this);

            if (producesPower && IsPowering)
                electricityinfo.Capacity += powerProduction;
        }

        private void OnDisableElectricity()
        {
            if (costsPower)
                electricityinfo.RemoveElectricityConsumption(this);

            if (producesPower && IsPowering)
                electricityinfo.Capacity -= powerProduction;
        }

        public void PowerOverloaded()
        {
            IsPowered = false;
        }

        #endregion

    }
}