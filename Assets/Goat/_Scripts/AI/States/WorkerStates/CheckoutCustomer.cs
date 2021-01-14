using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.AI.States
{
    /// <summary>
    /// Template to checkout customer for cashiere AI
    /// </summary>
    public class CheckoutCustomer : IState
    {
        private Cashiere cashiere;

        public CheckoutCustomer(Cashiere cashiere)
        {
            this.cashiere = cashiere;
        }

        public void Tick()
        {
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }
    }
}

