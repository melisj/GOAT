﻿using Goat.Grid.UI;
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
    public class InteractableInfo : Attribute {

        public string customName;

        public InteractableInfo(string customName = "") {
            this.customName = customName;
        }
    }

    /// <summary>
    /// Base script for every interactable object in the game
    /// Contains information of the object
    /// </summary>
    public class BaseInteractable : MonoBehaviour
    {
        [TextArea]
        [SerializeField] private string description;

        protected UnityEvent InformationChanged = new UnityEvent();

        protected virtual void OnEnable() {
            InteractableManager.InteractableClickEvt += IsClicked;
            InformationChanged.AddListener(UpdateUI);
            InformationChanged.AddListener(OpenUI);
        }

        protected virtual void OnDisable() {
            InteractableManager.InteractableClickEvt -= IsClicked;
            InformationChanged.RemoveAllListeners();
        }

        // Get the event when the object has been clicked
        // If clicked then open UI
        protected virtual void IsClicked(Transform clickedObj) {
            if (clickedObj == transform) {
                InvokeChange();
                InteractableManager.ChangeSelectedInteractable(this);
            }
        }

        // Open the UI for the this 
        public virtual void OpenUI() {
            GridUIManager.ShowNewUI(InteractableManager.instance.interactableUI);
        }

        // Hide this UI
        public virtual void CloseUI() {
            GridUIManager.HideUI();
        }

        // Update the UI when something has changed
        protected virtual void InvokeChange() {
            InformationChanged.Invoke();
        }

        // Update all the variables of the UI
        protected virtual void UpdateUI() {
            InteractableManager.instance.interactableUI.UnloadElement();
            InteractableManager.instance.interactableUI.SetUI(name, description, PrintObject<BaseInteractable>());
        }

        // Print out all the variables tagged with "InteractableInfo"
        private string PrintObject<T>() {
            string infoList = "";

            FieldInfo[] fields = typeof(T).GetFields();
            foreach (FieldInfo field in fields) {
                InteractableInfo meta = (InteractableInfo)field.GetCustomAttribute(typeof(InteractableInfo), true);
                if(meta != null)
                    infoList += field.Name + " - " + field.GetValue(this).ToString() + "\n";
            }

            return infoList;
        }
    }
}