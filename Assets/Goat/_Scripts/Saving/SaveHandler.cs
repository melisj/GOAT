using System;
using System.Linq;
using UnityEngine;

namespace Goat.Saving
{
    public class SaveHandler : MonoBehaviour
    {
        protected DataContainer data;

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

        public virtual void LoadEvent(SaveData data)
        {
            try
            {
                Load(data.data.First(x => x.className == this.data.className));
            }
            catch (Exception e)
            {
                Debug.LogWarningFormat("Handler: {0} has failed to get its corrosponding container, aborting loading! {1}", GetType().Name, e);
            }
            finally
            {
                Debug.LogFormat("Handler: {0} has checked for its data container.", GetType().Name);
            }
        }


        protected virtual void AddContainerToSave(SaveData data)
        {
            Save();
            data.AddData(this.data);
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