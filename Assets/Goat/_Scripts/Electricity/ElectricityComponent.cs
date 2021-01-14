using Sirenix.OdinInspector;
using UnityEngine;

namespace Goat.Farming.Electricity
{
    // Component can be added to any object that needs to consume or produce electricity
    // Component can call other scripts with event to change the powered state
    public class ElectricityComponent : MonoBehaviour
    {
        [SerializeField] private Electricity electricityinfo;

        [Header("Power Settings")]
        [SerializeField] private bool costsPower;
        [SerializeField, ShowIf("costsPower")] private int powerCost;
        [SerializeField, ShowIf("costsPower")] private bool isPowered;

        [SerializeField] private bool producesPower;
        [SerializeField, ShowIf("producesPower")] private int powerProduction;
        [SerializeField, ShowIf("producesPower")] private bool isPowering;

        public delegate void PoweredChangedEvent(bool powered);
        public event PoweredChangedEvent PoweredChangedEvt;

        public delegate void PoweringChangedEvent(bool powered);
        public event PoweringChangedEvent PoweringChangedEvt;

        public bool IsPowered
        {
            get { return isPowered; }
            set { isPowered = value; if (isPowered != value) PoweredChangedEvt?.Invoke(isPowered); }
        }

        public bool IsPowering
        {
            get { return isPowering; }
            set { isPowering = value; if (isPowering != value) PoweringChangedEvt?.Invoke(isPowering); }
        }

        public int PowerCost => powerCost;
        public int PowerProduction => powerProduction;

        private void OnEnable()
        {
            SetupElectricity();
        }

        private void OnDisable()
        {
            OnDisableElectricity();
        }

        private void SetupElectricity()
        {
            if (costsPower)
                electricityinfo.AddDevice(this);

            if (producesPower && IsPowering)
                electricityinfo.AddGenerator(this);
        }

        private void OnDisableElectricity()
        {
            if (costsPower)
                electricityinfo.RemoveDevice(this);

            if (producesPower && IsPowering)
                electricityinfo.RemoveGenerator(this);
        }
    }
}