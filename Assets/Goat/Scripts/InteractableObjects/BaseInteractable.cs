using Goat.Grid.UI;
using Goat.Pooling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

        [TextArea, Space(10)]
        [SerializeField] protected string description;

        protected Vector2Int gridPosition;

        protected Collider clickCollider;

        protected UnityEvent InformationChanged = new UnityEvent();

        public bool IsClickedOn { get; set; }
        public string Description => description;
        public string Name => name;
        public Vector2Int GridPosition { get { return gridPosition; } set { gridPosition = value; } }

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
            info.CurrentSelected = this;
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
            InformationChanged.Invoke();
        }

        // Print out all the variables tagged with "InteractableInfo"
        public virtual string PrintObject<T>()
        {
            string infoList = "";

            FieldInfo[] fields = typeof(T).GetFields();
            foreach (FieldInfo field in fields)
            {
                InteractableAttribute meta = (InteractableAttribute)field.GetCustomAttribute(typeof(InteractableAttribute), true);
                if (meta != null)
                    infoList += field.Name + " - " + field.GetValue(this).ToString() + "\n";
            }

            return infoList;
        }

        public virtual void OnGetObject(ObjectInstance objectInstance, int poolKey) {
            ObjInstance = objectInstance;
            PoolKey = poolKey;

            InteractableManager.InteractableClickEvt += IsClicked;
        }

        public virtual void OnReturnObject() {
            gameObject.transform.position = new Vector3(-1000, 0);
            gameObject.SetActive(false);

            InteractableManager.InteractableClickEvt -= IsClicked;
            InformationChanged.RemoveAllListeners();
        }
    }
}