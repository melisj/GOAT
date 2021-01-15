using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Goat.Saving
{
    public class SaveHandler : MonoBehaviour
    {
        public DataContainer data;
        public int saveOrder;
        public float timeoutLoadSec = 10;

        public bool DataDoneLoading { get; set; }
        public DataHandler.ContainerExitCode ExitCode { get; set; }
        float timeLoading;

        public virtual IEnumerator Load(DataHandler handler, DataContainer data) 
        {
            DataDoneLoading = false;
            this.data = data;
            StartCoroutine(this.data.Load(this));

            // Wait untill is done loading or times out
            yield return new WaitUntil(() => 
            { 
                timeLoading += Time.deltaTime; 
                return DataDoneLoading || timeLoading > timeoutLoadSec; 
            });

            // Check if the loading was succesful
            handler.LoadContainerExitCode = timeLoading > timeoutLoadSec ? DataHandler.ContainerExitCode.Failure : ExitCode;
            if(handler.LoadContainerExitCode == DataHandler.ContainerExitCode.Failure) 
                Debug.LogErrorFormat("Loading {0} took too long: {1}", data, timeLoading);
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

        private void SaveEvent(SaveData data)
        {
            AddContainerToSave(data);
        }

        private void LoadEvent(DataHandler handler, DataContainer data)
        {
            if (this.data.className == data.className)
            {
                try
                {
                    StartCoroutine(Load(handler, data));
                }
                catch (Exception e)
                {
                    Debug.LogWarningFormat("DataContainer: {0} failed to load! {1}", data.className, e);
                    handler.LoadContainerExitCode = DataHandler.ContainerExitCode.Failure;
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

        public virtual IEnumerator Load(SaveHandler handler)
        {
            handler.DataDoneLoading = true;
            yield break;
        }

        public void DoneLoading(SaveHandler handler, DataHandler.ContainerExitCode exitCode)
        {
            handler.DataDoneLoading = true;
            handler.ExitCode = exitCode;
        }

        public virtual void Save(SaveHandler handler)
        { }
    }

    public interface ISaveable
    {
        IEnumerator Load(SaveHandler handler);
        void DoneLoading(SaveHandler handler, DataHandler.ContainerExitCode exitCode);
        void Save(SaveHandler handler);
    }
}