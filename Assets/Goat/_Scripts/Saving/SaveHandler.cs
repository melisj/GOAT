using System;
using System.Linq;
using UnityEngine;

namespace Goat.Saving
{
    public class SaveHandler : MonoBehaviour
    {
        public DataContainer data;
        public int saveOrder;

        public virtual void Load(DataContainer data) 
        {
            this.data = data;
            this.data.Load(this);
        }

        public virtual void Save() 
        { 
            data.Save(this); 
        }

        #region Subscription

        public void Subscribe()
        {
            DataHandler.StartSaveEvent += SaveEvent;
            DataHandler.StartLoadEvent += LoadEvent;
        }

        public void Unsubscribe()
        {
            DataHandler.StartSaveEvent -= SaveEvent;
            DataHandler.StartLoadEvent -= LoadEvent;
        }

        public virtual void SaveEvent(SaveData data)
        {
            AddContainerToSave(data);
        }

        public virtual void LoadEvent(DataContainer data)
        {
            if (this.data.className == data.className)
            {
                try
                {
                    Load(data);
                    Debug.LogFormat("DataContainer: {0} has been loaded!", data.className);
                }
                catch (Exception e)
                {
                    Debug.LogWarningFormat("DataContainer: {0} failed to load! {1}", data.className, e);
                }
            }
        }

        protected virtual void AddContainerToSave(SaveData data)
        {
            Save();
            data.AddData(this);
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        #endregion
    }

    [Serializable]
    public class DataContainer : ISaveable
    {
        public string className;

        public DataContainer()
        {
            className = GetType().Name;
        }

        public virtual void Load(SaveHandler handler) { }
        public virtual void Save(SaveHandler handler) { }
    }

    public interface ISaveable
    {
        void Load(SaveHandler handler);
        void Save(SaveHandler handler);
    }
}