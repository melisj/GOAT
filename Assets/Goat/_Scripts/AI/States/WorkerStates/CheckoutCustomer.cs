using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.AI.States
{
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

