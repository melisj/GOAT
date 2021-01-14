using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Farming.Electricity
{
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

        public EventHandler<bool> PowerChanged;

        public bool IsPowered
        {
            get { return isPowered; }
            set { isPowered = value; if (isPowered != value) PowerChanged?.Invoke(this, value); }
        }

        public bool IsPowering => isPowering;

        public int PowerCost => powerCost;
        public int PowerProduction => powerProduction;


        #region Electricity

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