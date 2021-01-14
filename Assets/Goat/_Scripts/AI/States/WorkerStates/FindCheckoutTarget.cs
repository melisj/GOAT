using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Grid.Interactions;


namespace Goat.AI.States
{
    /// <summary>
    /// Find a checkout target to move to for cashiere
    /// setup not done/implemented
    /// </summary>
    public class FindCheckoutTarget : IState
    {
        private Cashiere cashiere;

        public FindCheckoutTarget(Cashiere cashiere)
        {
            this.cashiere = cashiere;
        }

        private CheckoutInteractable FindCheckout()
        {
            return null;
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
