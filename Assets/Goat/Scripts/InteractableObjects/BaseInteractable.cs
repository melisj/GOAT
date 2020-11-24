using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GOAT.Grid.UI
{
    public class InteractableInfo : Attribute {

        public string customName;

        public InteractableInfo(string customName = "") {
            this.customName = customName;
        }
    }


    public class BaseInteractable : MonoBehaviour
    {
        [TextArea]
        [SerializeField] private string description;
        [InteractableInfo] public string testText;
        [InteractableInfo] public int testInt;

        protected delegate void InformationChangeEvent();
        protected event InformationChangeEvent InformationChanged;

        private void OnEnable() {
            InteractableManager.InteractableClickEvt += IsClicked;
            InformationChanged += UpdateUI;
        }

        private void OnDisable() {
            InteractableManager.InteractableClickEvt -= IsClicked;
            InformationChanged -= UpdateUI;
        }

        protected virtual void IsClicked(Transform clickedObj) {
            if (clickedObj == transform) {
                OpenUI();
            }
        }

        public virtual void OpenUI() {
            GridUIManager.ShowNewUI(InteractableManager.instance.interactableUI);
            InvokeChange();
        }

        public virtual void CloseUI() {
            GridUIManager.HideUI();
        }

        protected virtual void InvokeChange() {
            InformationChanged.Invoke();
        }

        protected virtual void UpdateUI() {
            InteractableManager.instance.interactableUI.SetUI(name, description, PrintObject<BaseInteractable>());
        }

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