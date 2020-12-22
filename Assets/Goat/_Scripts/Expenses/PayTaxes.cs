using UnityEngine;
using System.Collections;
using Goat.Events;
using UnityAtoms.BaseAtoms;

namespace Goat.Expenses
{
    public class PayTaxes : PayExpense
    {
        [SerializeField] private IntVariable profit;

        public override void OnEventRaised(int value)
        {
            Pay();
        }

        public override void OnFullPay()
        {
            throw new System.NotImplementedException();
        }

        public override void Pay()
        {
        }
    }
}